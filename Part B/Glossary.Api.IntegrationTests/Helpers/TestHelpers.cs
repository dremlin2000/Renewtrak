using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace Glossary.Api.IntegrationTests.Helpers
{
    public static class TestHelpers
    {
        public static string SerializeObject(object dataObject) =>
            JsonConvert.SerializeObject(
                    dataObject,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        }
                    });

        public static StringContent GetHttpContent(string json) =>
          new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}
