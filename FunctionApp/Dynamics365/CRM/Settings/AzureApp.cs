using System;
using System.Collections.Generic;
using System.Text;

namespace Plumsail.DataSource.Dynamics365.CRM.Settings
{
    public class AzureApp
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Tenant { get; set; }
        public string DynamicsUrl { get; set; }
    }
}
