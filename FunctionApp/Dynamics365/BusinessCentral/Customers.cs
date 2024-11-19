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
    public class Customers
    {
        private readonly Settings.Customers _settings;
        private readonly GraphServiceClientProvider _graphProvider;
        private readonly ILogger<Customers> _logger;

        public Customers(IOptions<AppSettings> settings, GraphServiceClientProvider graphProvider, ILogger<Customers> logger)
        {
            _settings = settings.Value.Customers;
            _graphProvider = graphProvider;
            _logger = logger;
        }

        [Function("D365-BC-Customers")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("Dynamics365-BusinessCentral-Customers is requested.");

            var graph = await _graphProvider.Create();
            var company = await graph.GetCompanyAsync(_settings.Company);
            if (company == null)
            {
                return new NotFoundResult();
            }

            var customersPage = await graph.Financials.Companies[company.Id.Value].Customers.GetAsync();
            var customers = new List<Customer>();
            var pageIterator = PageIterator<Customer, CustomerCollectionResponse>
                .CreatePageIterator(graph, customersPage, customer =>
                {
                    customers.Add(customer);
                    return true;
                });

            await pageIterator.IterateAsync();
            return new OkObjectResult(customers);
        }
    }
}
