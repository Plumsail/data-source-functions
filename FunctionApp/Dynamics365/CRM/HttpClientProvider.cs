using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Plumsail.DataSource.Dynamics365.CRM.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            var credentials = new ClientCredential(_azureAppSettings.ClientId, _azureAppSettings.ClientSecret);
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{_azureAppSettings.Tenant}");
            var result = await authContext.AcquireTokenAsync(_azureAppSettings.DynamicsUrl, credentials);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
