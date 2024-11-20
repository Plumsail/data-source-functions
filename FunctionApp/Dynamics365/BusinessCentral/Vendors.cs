using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Beta.Models;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using Plumsail.DataSource.Dynamics365.CRM;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class Vendors
    {
        private readonly Settings.Vendors _settings;
        private readonly HttpClientProvider _httpClientProvider;
        private readonly ILogger<Vendors> _logger;

        public Vendors(IOptions<AppSettings> settings, HttpClientProvider httpClientProvider, ILogger<Vendors> logger)
        {
            _settings = settings.Value.Vendors;
            _httpClientProvider = httpClientProvider;
            _logger = logger;
        }

        [Function("D365-BC-Vendors")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("Dynamics365-BusinessCentral-Vendors is requested.");

            var client = _httpClientProvider.Create();
            var companyId = await client.GetCompanyIdAsync(_settings.Company);
            if (companyId == null)
            {
                return new NotFoundResult();
            }

            var vendorsJson = await client.GetStringAsync($"companies({companyId})/vendors");
            var vendors = JsonValue.Parse(vendorsJson);
            return new OkObjectResult(vendors?["value"]);
        }
    }
}
