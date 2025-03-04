namespace Plumsail.DataSource.Dynamics365.CRM.Settings
{
    public class AzureApp
    {
        public static readonly string CacheFileDir = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "data", "Dynamics365.CRM");
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Tenant { get; set; }
        public string DynamicsUrl { get; set; }
    }
}
