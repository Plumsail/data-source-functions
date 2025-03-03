using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace Plumsail.DataSource.Dynamics365.CRM
{
    public class Contacts(HttpClientProvider httpClientProvider, ILogger<Contacts> logger)
    {
        [Function("D365-CRM-Contacts")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "crm/contacts/{id?}")] HttpRequest req, Guid? id)
        {
            logger.LogInformation("Dynamics365-CRM-Contacts is requested.");

            try
            {
                var client = httpClientProvider.Create();

                if (!id.HasValue)
                {
                    var contactsJson = await client.GetStringAsync("contacts?$select=contactid,fullname");
                    var contacts = JsonValue.Parse(contactsJson);
                    return new OkObjectResult(contacts?["value"]);
                }

                var contactResponse = await client.GetAsync($"contacts({id})");
                if (!contactResponse.IsSuccessStatusCode)
                {
                    if (contactResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new NotFoundResult();
                    }

                    // throws Exception
                    contactResponse.EnsureSuccessStatusCode();
                }

                var contactJson = await contactResponse.Content.ReadAsStringAsync();
                return new ContentResult()
                {
                    Content = contactJson,
                    ContentType = "application/json"
                };
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An error has occured while processing Dynamics365-CRM-Contacts request.");
                return new StatusCodeResult(ex.StatusCode.HasValue ? (int)ex.StatusCode.Value : StatusCodes.Status500InternalServerError);
            }
        }
    }
}
