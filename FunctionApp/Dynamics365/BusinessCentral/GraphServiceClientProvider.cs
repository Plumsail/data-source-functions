using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    public class GraphServiceClientProvider
    {
        private readonly AzureApp _azureAppSettings;

        public GraphServiceClientProvider(IOptions<AppSettings> settings)
        {
            _azureAppSettings = settings.Value.AzureApp;
        }

        public async Task<GraphServiceClient> Create()
        {
            var app = ConfidentialClientApplicationBuilder.Create(_azureAppSettings.ClientId)
                .WithClientSecret(_azureAppSettings.ClientSecret)
                .WithTenantId(_azureAppSettings.Tenant)
                .Build();

            TokenCacheHelper.EnableSerialization(app.UserTokenCache);

            var accounts = await app.GetAccountsAsync();

            var authProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                var result = await app.AcquireTokenSilent(new string[] { "https://graph.microsoft.com/.default" }, accounts.FirstOrDefault()).ExecuteAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);
            });

            return new GraphServiceClient(authProvider);
        }
    }
}
