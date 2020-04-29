using System;
using System.Collections.Generic;
using System.Text;

namespace Plumsail.DataSource.SharePointList
{
    public class Settings
    {
        public string SiteUrl { get; set; }
        public string ListName { get; set; }
        public AzureApp AzureApp { get; set; }
    }

    public class AzureApp
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Tenant { get; set; }
    }
}
