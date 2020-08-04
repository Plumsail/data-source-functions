using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Plumsail.DataSource.SharePoint.Settings;

namespace Plumsail.DataSource.SharePoint
{
    public class ListData
    {
        private readonly GraphServiceClientProvider _graphProvider;
        private readonly Settings.ListData _settings;

        public ListData(IOptions<AppSettings> settings, GraphServiceClientProvider graphProvider)
        {
            _settings = settings.Value.ListData;
            _graphProvider = graphProvider;
        }

        [FunctionName("SharePoint-ListData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ListData is requested.");

            var graph = _graphProvider.Create();
            var list = await graph.GetListAsync(_settings.SiteUrl, _settings.ListName);

            var queryOptions = new List<QueryOption>
            {
                new QueryOption("select", "id"),
                new QueryOption("filter", $"fields/Title eq {req.Query["Location"]}"),
                new QueryOption("expand", "fields(select=Title,Author,Location)")
            };
            var itemsPage = await list.Items
                .Request(queryOptions)
                .GetAsync();
            var items = new List<ListItem>(itemsPage);

            while (itemsPage.NextPageRequest != null)
            {
                itemsPage = await itemsPage.NextPageRequest.GetAsync();
                items.AddRange(itemsPage);
            }

            return new OkObjectResult(items);
        }
    }
}