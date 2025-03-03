namespace Plumsail.DataSource.Dynamics365.BusinessCentral.Settings
{
    public class AzureApp
    {
        public static readonly string CacheFileDir = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "data", "Dynamics365.BusinessCentral");
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Tenant { get; set; }
        public string InstanceId { get; set; }
    }
}
