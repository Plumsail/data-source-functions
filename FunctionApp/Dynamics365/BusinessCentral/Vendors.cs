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
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class Vendors
    {
        private readonly Settings.Vendors _settings;
        private readonly GraphServiceClientProvider _graphProvider;
        private readonly ILogger<Vendors> _logger;

        public Vendors(IOptions<AppSettings> settings, GraphServiceClientProvider graphProvider, ILogger<Vendors> logger)
        {
            _settings = settings.Value.Vendors;
            _graphProvider = graphProvider;
            _logger = logger;
        }

        [Function("D365-BC-Vendors")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("Dynamics365-BusinessCentral-Vendors is requested.");

            var graph = await _graphProvider.Create();
            var company = await graph.GetCompanyAsync(_settings.Company);
            if (company == null)
            {
                return new NotFoundResult();
            }


            var vendorsPage = await graph.Financials.Companies[company.Id.Value].Vendors.GetAsync();

            var vendors = new List<Vendor>();
            var pageIterator = PageIterator<Vendor, VendorCollectionResponse>
                .CreatePageIterator(graph, vendorsPage, vendor =>
                {
                    vendors.Add(vendor);
                    return true;
                });

            await pageIterator.IterateAsync();
            return new OkObjectResult(vendors);
        }
    }
}
