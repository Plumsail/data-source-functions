using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Plumsail.DataSource
{
    class TokenCacheHelper
    {
        private readonly string _cacheFileDir;
        private readonly string _cacheFilePath;

        public TokenCacheHelper(string cacheFileDir)
        {
            _cacheFileDir = Environment.ExpandEnvironmentVariables(cacheFileDir);
            _cacheFilePath = Path.Combine(_cacheFileDir, "msal.cache");
        }

        public void EnableSerialization(ITokenCache tokenCache)
        {
            tokenCache.SetBeforeAccessAsync(BeforeAccessNotification);
            tokenCache.SetAfterAccessAsync(AfterAccessNotification);
        }

        private Task BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (_cacheFilePath)
            {
                if (File.Exists(_cacheFilePath))
                {
                    args.TokenCache.DeserializeMsalV3(File.ReadAllBytes(_cacheFilePath));
                }
            }

            return Task.CompletedTask;
        }

        private Task AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (args.HasStateChanged)
            {
                lock (_cacheFilePath)
                {
                    if (!Directory.Exists(_cacheFileDir))
                    {
                        Directory.CreateDirectory(_cacheFileDir);
                    }

                    File.WriteAllBytes(_cacheFilePath, args.TokenCache.SerializeMsalV3());
                }
            }

            return Task.CompletedTask;
        }
    }
}
