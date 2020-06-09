using AutoMapper;
using Glossary.Api.IntegrationTests.Helpers;
using Glossary.Core.ViewModels;
using Glossary.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Glossary.Api.IntegrationTests
{
    public class GlossaryControllerTests : IClassFixture<GlossaryApiFactory<Startup>>
    {
        private readonly GlossaryApiFactory<Startup> _factory;
        private readonly IMapper _mapper;
        private readonly string apiEndPoint = "/api/glossary";

        public GlossaryControllerTests(GlossaryApiFactory<Startup> factory)
        {
            _factory = factory;
            _mapper = _factory.Services.GetService<IMapper>();
        }

        private HttpClient GetHttpClient(IEnumerable<Core.Models.Glossary> glossaryItems = null)
        {
            var factory = _factory;
            if (glossaryItems?.Any() == true)
            {
                return _factory
                    .WithWebHostBuilder(builder =>
                        {
                            builder.ConfigureServices(services =>
                            {
                                var serviceProvider = services.BuildServiceProvider();

                                using (var scope = serviceProvider.CreateScope())
                                {
                                    var scopedServices = scope.ServiceProvider;
                                    var db = scopedServices.GetRequiredService<GlossaryDbContext>();
                                    var logger = scopedServices.GetRequiredService<ILogger<GlossaryControllerTests>>();

                                    try
                                    {
                                        db.Set<Core.Models.Glossary>().AddRange(glossaryItems);
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex, "An error occurred seeding " +
                                            "the database with test messages. Error: {Message}",
                                            ex.Message);
                                    }
                                }
                            });
                        })
                    .CreateClient(
                        new WebApplicationFactoryClientOptions
                        {
                            AllowAutoRedirect = false
                        });
            }

            return 
                factory.CreateClient(
                    new WebApplicationFactoryClientOptions
                    {
                        AllowAutoRedirect = false
                    });
        }

        [Fact]
        public async Task Given_GlossaryApi_When_RequestGetAll_Then_AllExistingTermsShouldBeRetreivedBackInAlphabeticalOrder()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var response = await client.GetAsync(apiEndPoint);
            var actualJsonData = await response.Content.ReadAsStringAsync();
            var expectedData = _mapper.Map<IEnumerable<GlossaryVm>>(Utilities.GetSeedTestData().OrderBy(x => x.Term));
            var expectedJsonData = TestHelpers.SerializeObject(expectedData);
            var actualData = 
                JsonConvert.DeserializeObject<IEnumerable<GlossaryVm>>(actualJsonData)
                .Where(x => expectedData.Any(y => y.Id == x.Id));
            var filteredActualJsonData = TestHelpers.SerializeObject(actualData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedJsonData, filteredActualJsonData);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_RequestGetById_Then_TheRequstedTermShouldBeRetreivedBack()
        {
            //Arrange
            var testTerm = Utilities.GetSeedTestData().Last();
            var client = GetHttpClient();

            //Act
            var response = await client.GetAsync($"{apiEndPoint}/{testTerm.Id}");
            var actualJsonData = await response.Content.ReadAsStringAsync();
            var expectedData = _mapper.Map<GlossaryVm>(testTerm);
            var expectedJsonData = TestHelpers.SerializeObject(expectedData);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedJsonData, actualJsonData);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_RequestGetByNotExistingId_Then_NotFoundShouldBeRetreivedBack()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var response = await client.GetAsync($"{apiEndPoint}/abf37a50-b248-4aa5-a9b7-49d06a922df8");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_RequestGetByInvalidId_Then_BadRequestShouldBeRetreivedBack()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var response = await client.GetAsync($"/api/glossary/wrong-guid");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_AddNewGlossaryTermWithTheExistingId_Then_InternalServerErrorIsRaised()
        {
            //Arrange
            var testTermVm =
                new GlossaryVm
                {
                    Id = Utilities.GetSeedTestData().Last().Id,
                    Term = "TestTerm",
                    Definition = "TestDefinition"
                };
            var client = GetHttpClient();

            //Act
            var response = await client.PostAsync(apiEndPoint, TestHelpers.GetHttpContent(JsonConvert.SerializeObject(testTermVm)));

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_AddNewGlossaryTerm_Then_RequestShouldSucceed()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var json = JsonConvert.SerializeObject(new GlossaryVm { Id = Guid.NewGuid(), Term = "Term1", Definition = "Definition1"  });
            var response = await client.PostAsync(apiEndPoint, TestHelpers.GetHttpContent(json));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_AddNewInvalidGlossaryTerm_Then_BadRequestShouldBeRetreivedBack()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var emptyTerm = new GlossaryVm { };
            var json = JsonConvert.SerializeObject(emptyTerm);
            var data = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync(apiEndPoint, data);
            var actualJsonData = await response.Content.ReadAsStringAsync();
            var errorMessage = JsonConvert.DeserializeObject<ErrorMessageVm>(actualJsonData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(errorMessage.Errors, x => x.PropertyName.Equals(nameof(emptyTerm.Id), StringComparison.OrdinalIgnoreCase));
            Assert.Contains(errorMessage.Errors, x => x.PropertyName.Equals(nameof(emptyTerm.Term), StringComparison.OrdinalIgnoreCase));
            Assert.Contains(errorMessage.Errors, x => x.PropertyName.Equals(nameof(emptyTerm.Definition), StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Given_GlossaryApi_When_UpdateGlossaryTerm_Then_RequestShouldSucceed()
        {
            //Arrange
            var testTerm = 
                new Core.Models.Glossary
                { 
                    Id = Guid.NewGuid(),
                    Term = "TestTermToUpdate",
                    Definition = "TestDefinitionToUpdate"
                };
            var client = GetHttpClient(new[] { testTerm });
            var testTermVm = _mapper.Map<GlossaryVm>(testTerm);
            testTermVm.Term = $"MyNewTestTerm_{Guid.NewGuid()}";
            testTermVm.Definition = $"MyNewTestDefinition_{Guid.NewGuid()}";

            //Act
            var json = JsonConvert.SerializeObject(testTermVm);
            var response = await client.PutAsync(apiEndPoint, TestHelpers.GetHttpContent(json));
            var actualJsonData = await response.Content.ReadAsStringAsync();
            var expectedJsonData = TestHelpers.SerializeObject(testTermVm);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedJsonData, actualJsonData);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_UpdateGlossaryTerm_Then_BadRequestShouldBeRetreivedBack()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var emptyTerm = new GlossaryVm { };
            var json = JsonConvert.SerializeObject(emptyTerm);
            var response = await client.PutAsync(apiEndPoint, TestHelpers.GetHttpContent(json));
            var actualJsonData = await response.Content.ReadAsStringAsync();
            var errorMessage = JsonConvert.DeserializeObject<ErrorMessageVm>(actualJsonData);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(errorMessage.Errors, x => x.PropertyName == nameof(emptyTerm.Id));
            Assert.Contains(errorMessage.Errors, x => x.PropertyName == nameof(emptyTerm.Term));
            Assert.Contains(errorMessage.Errors, x => x.PropertyName == nameof(emptyTerm.Definition));
        }

        [Fact]
        public async Task Given_GlossaryApi_When_UpdateGlossaryTerm_Then_NotFoundShouldBeRetreivedBack()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var invalidTerm =
                new GlossaryVm
                {
                    Id = Guid.NewGuid(),
                    Term = $"MyNewTestTerm_{Guid.NewGuid()}",
                    Definition = $"MyNewTestDefinition_{Guid.NewGuid()}"
                };
            var json = JsonConvert.SerializeObject(invalidTerm);
            var response = await client.PutAsync(apiEndPoint, TestHelpers.GetHttpContent(json));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_DeleteGlossaryTerm_Then_RequestShouldSucceed()
        {
            //Arrange
            var testTerm =
                new Core.Models.Glossary
                {
                    Id = Guid.NewGuid(),
                    Term = "TestTermToDelete",
                    Definition = "TestDefinitionToDelete"
                };
            var client = GetHttpClient(new[] { testTerm });

            //Act
            var response = await client.DeleteAsync($"{apiEndPoint}/{testTerm.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_DeleteGlossaryTermWithInvalidId_Then_BadRequestShouldSucceed()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var response = await client.DeleteAsync($"{apiEndPoint}/wrong-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_DeleteGlossaryTermWithNotExistingId_Then_NotFoundShouldSucceed()
        {
            //Arrange
            var client = GetHttpClient();

            //Act
            var response = await client.DeleteAsync($"{apiEndPoint}/3d9d04e1-284d-4042-8de9-42934d92104d");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }        
    }
}
