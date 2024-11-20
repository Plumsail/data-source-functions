using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Beta.Models;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class Items
    {
        private readonly Settings.Items _settings;
        private readonly HttpClientProvider _httpClientProvider;
        private readonly ILogger<Items> _logger;

        public Items(IOptions<AppSettings> settings, HttpClientProvider httpClientProvider, ILogger<Items> logger)
        {
            _settings = settings.Value.Items;
            _httpClientProvider = httpClientProvider;
            _logger = logger;
        }

        [Function("D365-BC-Items")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Dynamics365-BusinessCentral-Items is requested.");

            var client = _httpClientProvider.Create();
            var companyId = await client.GetCompanyIdAsync(_settings.Company);
            if (companyId == null)
            {
                return new NotFoundResult();
            }

            var itemsJson = await client.GetStringAsync($"companies({companyId})/items");
            var items = JsonValue.Parse(itemsJson);
            return new OkObjectResult(items?["value"]);
        }
    }
}
