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
using Newtonsoft.Json.Linq;

namespace Plumsail.DataSource.Dynamics365.CRM
{
    public class Accounts
    {
        private readonly HttpClientProvider _httpClientProvider;

        public Accounts(HttpClientProvider httpClientProvider)
        {
            _httpClientProvider = httpClientProvider;
        }

        [FunctionName("D365-CRM-Accounts")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Dynamics365-CRM-Accounts is requested.");

            var client = _httpClientProvider.Create();
            var contactsJson = await client.GetStringAsync("accounts");
            var contacts = JObject.Parse(contactsJson);

            return new OkObjectResult(contacts["value"]);
        }
    }
}
