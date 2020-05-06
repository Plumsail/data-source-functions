using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plumsail.DataSource.Dynamics365.BusinessCentral.Settings;
using System;

[assembly: FunctionsStartup(typeof(Plumsail.DataSource.Dynamics365.BusinessCentral.Startup))]
namespace Plumsail.DataSource.Dynamics365.BusinessCentral
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

            builder.Services.Configure<AppSettings>(configuration.GetSection("Dynamics365.BusinessCentral"));
            builder.Services.AddTransient<GraphServiceClientProvider>();
        }
    }
}
