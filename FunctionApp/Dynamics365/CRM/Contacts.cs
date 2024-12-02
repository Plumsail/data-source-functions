using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace Plumsail.DataSource.Dynamics365.CRM
{
    public class Contacts
    {
        private readonly HttpClientProvider _httpClientProvider;
        private readonly ILogger<Contacts> _logger;

        public Contacts(HttpClientProvider httpClientProvider, ILogger<Contacts> logger)
        {
            _httpClientProvider = httpClientProvider;
            _logger = logger;
        }

        [Function("D365-CRM-Contacts")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("Dynamics365-CRM-Contacts is requested.");

            var client = _httpClientProvider.Create();
            var contactsJson = await client.GetStringAsync("contacts");
            var contacts = JsonValue.Parse(contactsJson);

            return new OkObjectResult(contacts?["value"]);
        }
    }
}
