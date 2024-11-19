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
    public class Items
    {
        private readonly Settings.Items _settings;
        private readonly GraphServiceClientProvider _graphProvider;
        private readonly ILogger<Items> _logger;

        public Items(IOptions<AppSettings> settings, GraphServiceClientProvider graphProvider, ILogger<Items> logger)
        {
            _settings = settings.Value.Items;
            _graphProvider = graphProvider;
            _logger = logger;
        }

        [Function("D365-BC-Items")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Dynamics365-BusinessCentral-Vendors is requested.");

            var graph = await _graphProvider.Create();
            var company = await graph.GetCompanyAsync(_settings.Company);
            if (company == null)
            {
                return new NotFoundResult();
            }

            var itemsPage = await graph.Financials.Companies[company.Id.Value].Items.GetAsync();
            var items = new List<Item>();
            var pageIterator = PageIterator<Item, ItemCollectionResponse>
                .CreatePageIterator(graph, itemsPage, item =>
                {
                    items.Add(item);
                    return true;
                });

            await pageIterator.IterateAsync();
            return new OkObjectResult(items);
        }
    }
}
