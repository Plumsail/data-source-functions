using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Plumsail.DataSource.SharePoint.Settings;
using System.Net.Http;

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

            // for debugging requests
            //var debugHandler = new DebugRequestHandler(new DebugResponseHandler());
            //var httpClient = new HttpClient(debugHandler);
            //return new GraphServiceClient(httpClient, clientSecretCredential);
            return new GraphServiceClient(clientSecretCredential);
        }
    }
}
