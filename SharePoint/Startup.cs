using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plumsail.DataSource.SharePoint.Settings;
using System;

[assembly: FunctionsStartup(typeof(Plumsail.DataSource.SharePoint.Startup))]
namespace Plumsail.DataSource.SharePoint
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();

            builder.Services.Configure<AppSettings>(configuration.GetSection("SharePoint"));
            builder.Services.AddTransient<GraphServiceClientProvider>();
        }
    }
}
