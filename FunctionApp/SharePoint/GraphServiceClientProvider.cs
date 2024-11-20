using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Plumsail.DataSource.SharePoint.Settings;

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
            // using Azure.Identity;
            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(_azureAppSettings.Tenant, _azureAppSettings.ClientId, _azureAppSettings.ClientSecret, options);
            return new GraphServiceClient(clientSecretCredential);
        }
    }
}
