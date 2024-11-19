using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.CRM
{
    public class Accounts
    {
        private readonly HttpClientProvider _httpClientProvider;
        private readonly ILogger<Accounts> _logger;

        public Accounts(HttpClientProvider httpClientProvider, ILogger<Accounts> logger)
        {
            _httpClientProvider = httpClientProvider;
            _logger = logger;
        }

        [Function("D365-CRM-Accounts")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("Dynamics365-CRM-Accounts is requested.");

            var client = _httpClientProvider.Create();
            var contactsJson = await client.GetStringAsync("accounts");
            var contacts = JObject.Parse(contactsJson);

            return new OkObjectResult(contacts["value"]);
        }
    }
}
