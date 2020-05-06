using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Microsoft.Graph.Auth;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Plumsail.DataSource.SharePoint
{
    public class ListData
    {
        private Settings.AppSettings _settings;

        public ListData(IOptions<Settings.AppSettings> settings)
        {
            _settings = settings.Value;
        }

        [FunctionName("SharePoint-ListData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ListData is requested.");

            var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_settings.AzureApp.ClientId)
                .WithClientSecret(_settings.AzureApp.ClientSecret)
                .WithTenantId(_settings.AzureApp.Tenant)
                .Build();

            var authProvider = new ClientCredentialProvider(confidentialClientApplication);
            var graph = new GraphServiceClient(authProvider);
            var list = await graph.GetListAsync(_settings.ListData.SiteUrl, _settings.ListData.ListName);

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("select", "id"),
                new QueryOption("expand", "fields(select=Title,Author)")
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
