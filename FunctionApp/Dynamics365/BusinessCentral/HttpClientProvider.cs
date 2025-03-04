using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System.Net.Http.Headers;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class HttpClientProvider(IOptions<AppSettings> settings)
    {
        private readonly AzureApp _azureAppSettings = settings.Value.AzureApp;

        public HttpClient Create()
        {
            // for debugging requests
            //var debugHandler = new DebugRequestHandler(new DebugResponseHandler());
            //var client = new HttpClient(new OAuthMessageHandler(_azureAppSettings, debugHandler));

            var client = new HttpClient(new OAuthMessageHandler(_azureAppSettings));
            client.BaseAddress = new Uri($"https://api.businesscentral.dynamics.com/v2.0/{_azureAppSettings.InstanceId}/Production/api/v2.0/");
            client.Timeout = new TimeSpan(0, 2, 0);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }

    class OAuthMessageHandler(AzureApp azureAppSettings, HttpMessageHandler? innerHandler = null)
        : DelegatingHandler(innerHandler ?? new HttpClientHandler())
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var app = ConfidentialClientApplicationBuilder.Create(azureAppSettings.ClientId)
               .WithClientSecret(azureAppSettings.ClientSecret)
               .WithTenantId(azureAppSettings.Tenant)
               .Build();

            var cache = new TokenCacheHelper(AzureApp.CacheFileDir);
            cache.EnableSerialization(app.UserTokenCache);

            var account = await app.GetAccountAsync(cache.GetAccountIdentifier());
            var result = await app.AcquireTokenSilent(["https://api.businesscentral.dynamics.com/.default"], account).ExecuteAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
