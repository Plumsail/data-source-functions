using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral
{
    class TokenCacheHelper
    {
        public static readonly string CacheFileDir = Environment.ExpandEnvironmentVariables(@"%HOME%\data\Dynamics365.BusinessCentral");
        public static readonly string CacheFilePath = Path.Combine(CacheFileDir, "msal.cache");
        private static readonly object FileLock = new object();

        public static void EnableSerialization(ITokenCache tokenCache)
        {
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
        }

        private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                if (File.Exists(CacheFilePath))
                {
                    args.TokenCache.DeserializeMsalV3(File.ReadAllBytes(CacheFilePath));
                }
            }
        }

        private static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (args.HasStateChanged)
            {
                lock (FileLock)
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
