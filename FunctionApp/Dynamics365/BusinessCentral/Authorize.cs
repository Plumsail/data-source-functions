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
    public class Authorize
    {
        private readonly AzureApp _settings;

        public Authorize(IOptions<AppSettings> settings)
        {
            _settings = settings.Value.AzureApp;
        }

        [FunctionName("D365-BC-Authorize")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var scopes = new string[] { "https://graph.microsoft.com/.default", "offline_access" };

            if (req.Method == "POST" && req.Form.ContainsKey("code"))
            {
                var code = req.Form["code"].FirstOrDefault();

                var app = ConfidentialClientApplicationBuilder.Create(_settings.ClientId)
                    .WithClientSecret(_settings.ClientSecret)
                    .WithTenantId(_settings.Tenant)
                    .WithRedirectUri(req.GetDisplayUrl())
                    .Build();

                var cache = new TokenCacheHelper(AzureApp.CacheFileDir);
                cache.EnableSerialization(app.UserTokenCache);

                _ = await app.AcquireTokenByAuthorizationCode(scopes, code).ExecuteAsync();

                return new OkObjectResult("The app is authorized to perform operations on behalf of your account.");
            }

            var url = new StringBuilder();
            url.Append($"https://login.microsoftonline.com/{_settings.Tenant}/oauth2/v2.0/authorize?");
            url.Append($"client_id={_settings.ClientId}&");
            url.Append($"response_type=code&");
            url.Append($"redirect_uri={req.GetEncodedUrl()}&");
            url.Append($"response_mode=form_post&");
            url.Append($"scope={WebUtility.UrlEncode(string.Join(" ", scopes))}&");
            return new RedirectResult(url.ToString(), false);
        }
    }
}
