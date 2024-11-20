using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

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
            var accountsJson = await client.GetStringAsync("accounts");
            var accounts = JsonValue.Parse(accountsJson);

            return new OkObjectResult(accounts?["value"]);
        }
    }
}
