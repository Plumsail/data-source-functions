using Microsoft.Identity.Client;
using System;
using System.IO;

namespace Plumsail.DataSource
{
    class TokenCacheHelper
    {
        public readonly string CacheFileDir;
        public readonly string CacheFilePath;

        public TokenCacheHelper(string cacheFileDir = @"%HOME%\data")
        {
            CacheFileDir = Environment.ExpandEnvironmentVariables(cacheFileDir);
            CacheFilePath = Path.Combine(CacheFileDir, "msal.cache");
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
                }
            }
        }
    }
}
