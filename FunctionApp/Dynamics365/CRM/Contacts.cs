using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Plumsail.DataSource.Dynamics365.CRM
{
    public class Contacts
    {
        private readonly HttpClientProvider _httpClientProvider;

        public Contacts(HttpClientProvider httpClientProvider)
        {
            _httpClientProvider = httpClientProvider;
        }

        [FunctionName("D365-CRM-Contacts")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Dynamics365-CRM-Contacts is requested.");

            var client = _httpClientProvider.Create();
            var contactsJson = await client.GetStringAsync("contacts");
            var contacts = JObject.Parse(contactsJson);

            return new OkObjectResult(contacts["value"]);
        }
    }
}
