using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Plumsail.DataSource.SharePoint
{
    public class ListData
    {
        private readonly Settings.ListData _settings;
        private readonly GraphServiceClientProvider _graphProvider;
        private readonly ILogger<ListData> _logger;

        public ListData(IOptions<Settings.AppSettings> settings, GraphServiceClientProvider graphProvider, ILogger<ListData> logger)
        {
            _logger = logger;
            _settings = settings.Value.ListData;
            _graphProvider = graphProvider;
        }

        [Function("SharePoint-ListData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "sp/items/{id?}")] HttpRequest req, int? id)
        {
            _logger.LogInformation("SharePoint-ListData is requested.");

            var graph = _graphProvider.Create();
            var list = await graph.GetListAsync(_settings.SiteUrl, _settings.ListName);
            if (list == null)
            {
                return new NotFoundResult();
            }

            if (!id.HasValue)
            {
                var itemsPage = await list.Items.GetAsync(requestConfiguration =>
                {
                    //requestConfiguration.QueryParameters.Filter = "fields/Title eq 'item 1'";
                    requestConfiguration.QueryParameters.Select = ["id"];
                    requestConfiguration.QueryParameters.Expand = ["fields($select=Title,Author)"];
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

            try 
            {
                var item = await list.Items[id.ToString()].GetAsync();
                return new OkObjectResult(item?.Fields?.AdditionalData);
            }
            catch (ODataError error) when (error.ResponseStatusCode.Equals(404))
            {
                return new NotFoundResult();
            }
        }
    }
}
