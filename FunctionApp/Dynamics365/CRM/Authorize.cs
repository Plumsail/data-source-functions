using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Plumsail.DataSource.Dynamics365.CRM.Settings;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.CRM
{
    public class Authorize
    {
        private readonly AzureApp _settings;

        public Authorize(IOptions<AppSettings> settings)
        {
            _settings = settings.Value.AzureApp;
        }

        [FunctionName("D365-CRM-Authorize")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var scopes = new string[] { "https://admin.services.crm.dynamics.com/user_impersonation", "offline_access" };

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
                
                _ = await app.AcquireTokenByAuthorizationCode(new string[] { $"{_settings.DynamicsUrl}/user_impersonation" }, code).ExecuteAsync();

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
