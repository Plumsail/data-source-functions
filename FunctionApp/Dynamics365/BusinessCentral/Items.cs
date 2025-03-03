using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System.Text.Json.Nodes;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class Items(IOptions<AppSettings> settings, HttpClientProvider httpClientProvider, ILogger<Items> logger)
    {
        private readonly Settings.Items _settings = settings.Value.Items;

        [Function("D365-BC-Items")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "bc/items/{id?}")] HttpRequest req, Guid? id)
        {
            logger.LogInformation("D365-BC-Items is requested.");

            try
            {
                var client = httpClientProvider.Create();
                var companyId = await client.GetCompanyIdAsync(_settings.Company);
                if (companyId == null)
                {
                    return new NotFoundResult();
                }

                if (!id.HasValue)
                {
                    var itemsJson = await client.GetStringAsync($"companies({companyId})/items");
                    var items = JsonValue.Parse(itemsJson);
                    return new OkObjectResult(items?["value"]);
                }

                var itemResponse = await client.GetAsync($"companies({companyId})/items({id})");
                if (!itemResponse.IsSuccessStatusCode)
                {
                    if (itemResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new NotFoundResult();
                    }

                    // throws Exception
                    itemResponse.EnsureSuccessStatusCode();
                }

                var itemJson = await itemResponse.Content.ReadAsStringAsync();
                return new ContentResult()
                {
                    Content = itemJson,
                    ContentType = "application/json"
                };
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "An error has occured while processing D365-BC-Items request.");
                return new StatusCodeResult(ex.StatusCode.HasValue ? (int)ex.StatusCode.Value : StatusCodes.Status500InternalServerError);
            }
        }
    }
}
