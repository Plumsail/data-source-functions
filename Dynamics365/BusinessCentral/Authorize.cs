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
        private AppSettings _settings;

        public Authorize(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        [FunctionName("Dynamics365-BusinessCentral-Authorize")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var scopes = new string[] { "https://graph.microsoft.com/.default", "offline_access" };
            var currentUrl = UriHelper.BuildAbsolute(req.Scheme, req.Host, req.PathBase, req.Path);

            var queryParams = req.GetQueryParameterDictionary();
            if (queryParams.ContainsKey("code"))
            {
                var code = queryParams["code"];

                var app = ConfidentialClientApplicationBuilder.Create(_settings.AzureApp.ClientId)
                    .WithClientSecret(_settings.AzureApp.ClientSecret)
                    .WithTenantId(_settings.AzureApp.Tenant)
                    .WithRedirectUri(currentUrl)
                    .Build();

                TokenCacheHelper.EnableSerialization(app.UserTokenCache);

                _ = await app.AcquireTokenByAuthorizationCode(scopes, code).ExecuteAsync();

                return new OkResult();
            }

            var url = new StringBuilder();
            url.Append($"https://login.microsoftonline.com/{_settings.AzureApp.Tenant}/oauth2/v2.0/authorize?");
            url.Append($"client_id={_settings.AzureApp.ClientId}&");
            url.Append($"response_type=code&");
            url.Append($"redirect_uri={WebUtility.UrlEncode(currentUrl)}&");
            url.Append($"response_mode=query&");
            url.Append($"scope={WebUtility.UrlEncode(string.Join(" ", scopes))}&");
            return new RedirectResult(url.ToString(), false);
        }
    }
}
