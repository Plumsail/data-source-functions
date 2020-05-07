using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    internal static class GraphServiceClientExtensions
    {
        internal static async System.Threading.Tasks.Task<Company> GetCompanyAsync(this GraphServiceClient graph, string companyName)
        {
            var companies = await graph.Financials.Companies.Request().Select(c => new { c.Id, c.Name, c.DisplayName }).GetAsync();
            var company = companies.FirstOrDefault(c => c.Name == companyName || c.DisplayName == companyName);
            return company;
        }
    }
}
