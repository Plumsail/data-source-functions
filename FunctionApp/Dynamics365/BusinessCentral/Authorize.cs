using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class Authorize
    {
        private readonly AzureApp _settings;
        private readonly ILogger<Authorize> _logger;

        public Authorize(IOptions<AppSettings> settings, ILogger<Authorize> logger)
        {
            _settings = settings.Value.AzureApp;
            _logger = logger;
        }

        [Function("D365-BC-Authorize")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
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
