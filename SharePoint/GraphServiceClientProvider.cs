using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Plumsail.DataSource.SharePoint.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Plumsail.DataSource.SharePoint
{
    public class GraphServiceClientProvider
    {
        private readonly AzureApp _azureAppSettings;

        public GraphServiceClientProvider(IOptions<AppSettings> settings)
        {
            _azureAppSettings = settings.Value.AzureApp;
        }

        public GraphServiceClient Create()
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_azureAppSettings.ClientId)
                .WithClientSecret(_azureAppSettings.ClientSecret)
                .WithTenantId(_azureAppSettings.Tenant)
                .Build();

            var authProvider = new ClientCredentialProvider(confidentialClientApplication);
            return new GraphServiceClient(authProvider);
        }
    }
}
