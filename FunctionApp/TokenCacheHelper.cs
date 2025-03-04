using Microsoft.Identity.Client;

namespace Plumsail.DataSource
{
    class TokenCacheHelper
    {
        public readonly string CacheFileDir;
        public readonly string CacheFilePath;
        public readonly string AccountFilePath;

        public TokenCacheHelper(string cacheFileDir)
        {
            CacheFileDir = Path.GetFullPath(Environment.ExpandEnvironmentVariables(cacheFileDir));
            CacheFilePath = Path.Combine(CacheFileDir, "msal.cache");
            AccountFilePath = Path.Combine(CacheFileDir, "msal-account.cache");
        }

        public string GetAccountIdentifier()
        {
            return File.ReadAllText(AccountFilePath);
        }

        public void EnableSerialization(ITokenCache tokenCache)
        {
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (CacheFilePath)
            {
                if (File.Exists(CacheFilePath))
                {
                    args.TokenCache.DeserializeMsalV3(File.ReadAllBytes(CacheFilePath));
                }
            }
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (args.HasStateChanged)
            {
                lock (CacheFilePath)
                {
                    if (!Directory.Exists(CacheFileDir))
                    {
                        Directory.CreateDirectory(CacheFileDir);
                    }

                    
                    File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
                    File.WriteAllText(AccountFilePath, args.Account.HomeAccountId.Identifier);
                }
            }
        }
    }
}
