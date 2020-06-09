using AutoMapper;
using Glossary.Api.Controllers;
using Glossary.Core.Abstract.Repositories;
using Glossary.Core.Automapper;
using Glossary.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Glossary.Api.Tests
{
    public class GlossaryControllerTests
    {
        private readonly GlossaryController _glossaryController;
        private readonly Mock<IGlossaryRepository> _mockGlossaryRepository = new Mock<IGlossaryRepository>();
        private readonly List<Core.Models.Glossary> _glossaryList = new List<Core.Models.Glossary>
            {
                new Core.Models.Glossary
                {
                    Id = Guid.Parse("f11e32e6-51dd-449a-9989-156f6c805960"),
                    Term = "abyssal plain",
                    Definition = "The ocean floor offshore from the continental margin, usually very flat with a slight slope."
                },
                new Core.Models.Glossary
                {
                    Id = Guid.Parse("b21ceca6-4143-4453-bb14-d189eb53e403"),
                    Term = "accrete",
                    Definition = "To add terranes (small land masses or pieces of crust) to another, usually larger, land mass."
                }
            };

        public GlossaryControllerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AppProfile>();
            });

            var mapper = config.CreateMapper();
            _glossaryController = new GlossaryController(_mockGlossaryRepository.Object, mapper);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_RequestGetAll_Then_AllGlossaryItemsShoudBeRetrieved()
        {
            //Arrange
            _mockGlossaryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(_glossaryList);

            //Act
            var result = await _glossaryController.GetAll();

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var actualList = okObjectResult.Value as IEnumerable<GlossaryVm>;

            var expectedList = _glossaryList
                .Select(x => new GlossaryVm
                {
                    Id = x.Id,
                    Term = x.Term,
                    Definition = x.Definition
                })
                .OrderBy(x => x.Id);
            Assert.Equal(JsonConvert.SerializeObject(expectedList), JsonConvert.SerializeObject(actualList.OrderBy(x => x.Id)));
        }

        [Fact]
        public async Task Given_GlossaryApi_When_RequestGetById_Then_TheRequestedTermShouldBeRetrievedBack()
        {
            //Arrange
            var glossaryItem = _glossaryList.Last();
            _mockGlossaryRepository.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(glossaryItem);

            var expectedItem = new GlossaryVm
            {
                Id = glossaryItem.Id,
                Term = glossaryItem.Term,
                Definition = glossaryItem.Definition
            };

            //Act
            var result = await _glossaryController.GetById(glossaryItem.Id);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var actualCar = okObjectResult.Value as GlossaryVm;
            Assert.Equal(JsonConvert.SerializeObject(expectedItem), JsonConvert.SerializeObject(actualCar));
        }

        [Fact]
        public async Task Given_GlossaryApi_When_RequestGetByNotExistingId_Then_NotFoundShouldBeRetrievedBack()
        {
            //Arrange
            _mockGlossaryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(_glossaryList);

            //Act
            var result = await _glossaryController.GetById(Guid.NewGuid());

            //Assert
            var okObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_AddANewItem_Then_ItShouldSucceedAndReturnTheNewItem()
        {
            _mockGlossaryRepository
                .Setup(x => x.Add(It.IsAny<Core.Models.Glossary>()));

            //Act
            var expectedCarVm = new GlossaryVm
            {
                Id = Guid.NewGuid(),
                Term = "TestTerm",
                Definition = "TestDefinition"
            };
            var result = await _glossaryController.Add(expectedCarVm);

            //Assert
            var objectResult = Assert.IsType<CreatedAtRouteResult>(result);
            var actualVm = objectResult.Value as GlossaryVm;
            Assert.Equal(JsonConvert.SerializeObject(expectedCarVm), JsonConvert.SerializeObject(actualVm));
        }

        [Fact]
        public async Task Given_GlossaryApi_When_Update_Then_ItShouldSucceedAndReturnTheUpdateCarBack()
        {
            var testModel = _glossaryList.Last();
            _mockGlossaryRepository
                .Setup(x => x.GetAsync(It.Is<Guid>(id => id == testModel.Id)))
                .ReturnsAsync(testModel);
            _mockGlossaryRepository
                .Setup(x => x.Update(It.IsAny<Core.Models.Glossary>()));

            //Act
            var expectedVm = new GlossaryVm
            {
                Id = testModel.Id,
                Term = "UpdateTerm1",
                Definition = "UpdateDefinition1",
            };
            var result = await _glossaryController.Update(expectedVm);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var actualVm = okObjectResult.Value as GlossaryVm;

            Assert.Equal(JsonConvert.SerializeObject(expectedVm), JsonConvert.SerializeObject(actualVm));
            _mockGlossaryRepository.Verify(
                repo => repo.Update(It.Is<Core.Models.Glossary>(model => model.Id == testModel.Id)), Times.Once);
        }

        [Fact]
        public async Task Given_GlossaryApi_When_Delete_Then_ItShouldSucceed()
        {
            var testModel = _glossaryList.Last();
            _mockGlossaryRepository
                .Setup(x => x.GetAsync(It.Is<Guid>(id => id == testModel.Id)))
                .ReturnsAsync(testModel);
            _mockGlossaryRepository
                .Setup(x => x.DeleteAsync(It.IsAny<Guid>()));

            //Act
            var result = await _glossaryController.Delete(testModel.Id);

            //Assert
            var okObjectResult = Assert.IsType<NoContentResult>(result);


            _mockGlossaryRepository.Verify(
                repo => repo.DeleteAsync(It.Is<Guid>(id => id == testModel.Id)), Times.Once);
        }
    }
}
