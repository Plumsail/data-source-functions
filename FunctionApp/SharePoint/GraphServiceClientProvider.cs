using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Plumsail.DataSource.SharePoint.Settings;

namespace Plumsail.DataSource.SharePoint
{
    public class GraphServiceClientProvider(IOptions<AppSettings> settings)
    {
        private readonly AzureApp _azureAppSettings = settings.Value.AzureApp;

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
