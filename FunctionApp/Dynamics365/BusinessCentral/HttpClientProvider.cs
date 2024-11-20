using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using Plumsail.DataSource.Dynamics365.CRM;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class HttpClientProvider
    {
        private readonly AzureApp _azureAppSettings;

        public HttpClientProvider(IOptions<AppSettings> settings)
        {
            _azureAppSettings = settings.Value.AzureApp;
        }

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

    class OAuthMessageHandler : DelegatingHandler
    {
        private readonly AzureApp _azureAppSettings;

        public OAuthMessageHandler(AzureApp azureAppSettings, HttpMessageHandler? innerHandler = null) : base(innerHandler ?? new HttpClientHandler())
        {
            _azureAppSettings = azureAppSettings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var app = ConfidentialClientApplicationBuilder.Create(_azureAppSettings.ClientId)
               .WithClientSecret(_azureAppSettings.ClientSecret)
               .WithTenantId(_azureAppSettings.Tenant)
               .Build();

            var cache = new TokenCacheHelper(AzureApp.CacheFileDir);
            cache.EnableSerialization(app.UserTokenCache);

            var account = await app.GetAccountAsync(cache.GetAccountIdentifier());
            var result = await app.AcquireTokenSilent(["https://api.businesscentral.dynamics.com/.default"], account).ExecuteAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
