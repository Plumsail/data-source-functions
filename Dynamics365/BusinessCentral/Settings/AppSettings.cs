using System;
using System.Collections.Generic;
using System.Text;

namespace Plumsail.DataSource.Dynamics365.BusinessCentral.Settings 
{
    public class AppSettings
    {
        public AzureApp AzureApp { get; set; }
        public Customers Customers { get; set; }
        public Vendors Vendors { get; set; }
    }
}
