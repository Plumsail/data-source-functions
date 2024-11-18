using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using System.Linq;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    internal static class GraphServiceClientExtensions
    {
        internal static async System.Threading.Tasks.Task<Company> GetCompanyAsync(this GraphServiceClient graph, string companyName)
        {
            var companies = await graph.Financials.Companies.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = ["Id", "Name", "DisplayName"];
            });

            var company = companies.Value?.FirstOrDefault(c => c.Name == companyName || c.DisplayName == companyName);
            return company;
        }
    }
}
