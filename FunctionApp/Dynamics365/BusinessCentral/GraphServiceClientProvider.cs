using Azure.Core;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.Identity.Client;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System;
using System.Threading;
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

            var cache = new TokenCacheHelper(AzureApp.CacheFileDir);
            cache.EnableSerialization(app.UserTokenCache);
            var account = await app.GetAccountAsync(cache.GetAccountIdentifier());
            var result = await app.AcquireTokenSilent(["https://graph.microsoft.com/.default"], account).ExecuteAsync();

            return new GraphServiceClient(new TokenProvider(result.AccessToken, result.ExpiresOn));
        }

        internal class TokenProvider(string token, DateTimeOffset expiresOn) : TokenCredential
        {
            public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                return GetTokenAsync(requestContext, cancellationToken).Result;
            }

            public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                return ValueTask.FromResult(new AccessToken(token, expiresOn));
            }
        }
    }
}
