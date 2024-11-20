using System.Text.Json.Nodes;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    internal static class HttpClientExtensions
    {
        internal static async System.Threading.Tasks.Task<string> GetCompanyIdAsync(this HttpClient client, string companyName)
        {
            var companiesJson = await client.GetStringAsync("companies?$select=id,name");
            var contacts = JsonValue.Parse(companiesJson)["value"].AsArray();

            return contacts.FirstOrDefault(c => c["name"]?.GetValue<string>() == companyName)?["id"]?.GetValue<string>();
        }
    }
}
