using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plumsail.DataSource.SharePoint
{
    public class ListData
    {
        private readonly Settings.ListData _settings;
        private readonly GraphServiceClientProvider _graphProvider;

        public ListData(IOptions<Settings.AppSettings> settings, GraphServiceClientProvider graphProvider)
        {
            _settings = settings.Value.ListData;
            _graphProvider = graphProvider;
        }

        [FunctionName("SharePoint-ListData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ListData is requested.");

            var graph = _graphProvider.Create();
            var list = await graph.GetListAsync(_settings.SiteUrl, _settings.ListName);
            var itemsPage = await list.Items.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = ["id"];
                requestConfiguration.QueryParameters.Expand = ["fields(select=Title,Author)"];
            });

            var items = new List<ListItem>();
            var pageIterator = PageIterator<ListItem, ListItemCollectionResponse>
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
