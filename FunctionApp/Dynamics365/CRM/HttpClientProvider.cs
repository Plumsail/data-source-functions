using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Plumsail.DataSource.Dynamics365.CRM.Settings;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.CRM
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
            var client = new HttpClient(new OAuthMessageHandler(_azureAppSettings, new HttpClientHandler()));
            client.BaseAddress = new Uri($"{_azureAppSettings.DynamicsUrl}/api/data/v9.1/");
            client.Timeout = new TimeSpan(0, 2, 0);
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }

    class OAuthMessageHandler : DelegatingHandler
    {
        private readonly AzureApp _azureAppSettings;

        public OAuthMessageHandler(AzureApp azureAppSettings, HttpMessageHandler innerHandler) : base(innerHandler)
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

            var accounts = await app.GetAccountsAsync();
            var result = await app.AcquireTokenSilent(new string[] { $"{_azureAppSettings.DynamicsUrl}/user_impersonation" }, accounts.FirstOrDefault()).ExecuteAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
