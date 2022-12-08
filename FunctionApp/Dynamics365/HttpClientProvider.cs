using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Plumsail.DataSource.Dynamics365.Settings;

namespace Plumsail.DataSource.Dynamics365
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
            client.BaseAddress = new Uri($"{_azureAppSettings.DynamicsUrl}/api/data/v9.2/");
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
        private readonly IConfidentialClientApplication _app;

        public OAuthMessageHandler(AzureApp azureAppSettings, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _azureAppSettings = azureAppSettings;

            _app = ConfidentialClientApplicationBuilder.Create(_azureAppSettings.ClientId)
                .WithClientSecret(_azureAppSettings.ClientSecret)
                .WithTenantId(_azureAppSettings.Tenant)
                .WithLegacyCacheCompatibility(false)
                .Build();

            new TokenCacheHelper(AppDomain.CurrentDomain.BaseDirectory).EnableSerialization(_app.AppTokenCache);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var scopes = new[] { $"{_azureAppSettings.DynamicsUrl}/.default" };
            var accessToken = (await _app.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken)).AccessToken;

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
