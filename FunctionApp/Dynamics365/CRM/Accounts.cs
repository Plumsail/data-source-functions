using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace Plumsail.DataSource.Dynamics365.CRM
{
    public class Accounts(HttpClientProvider httpClientProvider, ILogger<Accounts> logger)
    {
        [Function("D365-CRM-Accounts")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "crm/accounts/{id?}")] HttpRequest req, Guid? id)
        {
            logger.LogInformation("Dynamics365-CRM-Accounts is requested.");

            try
            {
                var client = httpClientProvider.Create();

                if (!id.HasValue)
                {
                    var accountsJson = await client.GetStringAsync("accounts?$select=accountid,name");
                    var accounts = JsonValue.Parse(accountsJson);
                    return new OkObjectResult(accounts?["value"]);
                }

                var accountResponse = await client.GetAsync($"accounts({id})");
                if (!accountResponse.IsSuccessStatusCode)
                {
                    if (accountResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new NotFoundResult();
                    }

                    // throws Exception
                    accountResponse.EnsureSuccessStatusCode();
                }

                var accountJson = await accountResponse.Content.ReadAsStringAsync();
                return new ContentResult()
                {
                    Content = accountJson,
                    ContentType = "application/json"
                };
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An error has occured while processing Dynamics365-CRM-Accounts request.");
                return new StatusCodeResult(ex.StatusCode.HasValue ? (int)ex.StatusCode.Value : StatusCodes.Status500InternalServerError);
            }
        }
    }
}
