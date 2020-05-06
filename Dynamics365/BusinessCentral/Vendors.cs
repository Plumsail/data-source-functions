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
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class Vendors
    {
        private Settings.Vendors _settings;
        private GraphServiceClientProvider _graphProvider;

        public Vendors(IOptions<AppSettings> settings, GraphServiceClientProvider graphProvider)
        {
            _settings = settings.Value.Vendors;
            _graphProvider = graphProvider;
        }

        [FunctionName("Dynamics365-BusinessCentral-Vendors")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Dynamics365-BusinessCentral-Vendors is requested.");

            var graph = await _graphProvider.Create();
            var company = await graph.GetCompanyAsync(_settings.Company);
            if (company == null)
            {
                return new NotFoundResult();
            }

            var vendorsPage = await graph.Financials.Companies[company.Id].Vendors.Request().GetAsync();
            var vendors = new List<Vendor>(vendorsPage);
            while (vendorsPage.NextPageRequest != null)
            {
                vendorsPage = await vendorsPage.NextPageRequest.GetAsync();
                vendors.AddRange(vendorsPage);
            }

            return new OkObjectResult(vendors);
        }
    }
}
