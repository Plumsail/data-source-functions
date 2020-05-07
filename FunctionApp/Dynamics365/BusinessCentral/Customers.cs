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
    public class Customers
    {
        private readonly Settings.Customers _settings;
        private readonly GraphServiceClientProvider _graphProvider;

        public Customers(IOptions<AppSettings> settings, GraphServiceClientProvider graphProvider)
        {
            _settings = settings.Value.Customers;
            _graphProvider = graphProvider;
        }

        [FunctionName("D365-BC-Customers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Dynamics365-BusinessCentral-Customers is requested.");

            var graph = await _graphProvider.Create();
            var company = await graph.GetCompanyAsync(_settings.Company);
            if (company == null)
            {
                return new NotFoundResult();
            }

            var customersPage = await graph.Financials.Companies[company.Id].Customers.Request().GetAsync();
            var customers = new List<Customer>(customersPage);
            while (customersPage.NextPageRequest != null)
            {
                customersPage = await customersPage.NextPageRequest.GetAsync();
                customers.AddRange(customersPage);
            }

            return new OkObjectResult(customers);
        }
    }
}
