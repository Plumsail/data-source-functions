using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plumsail.DataSource;
using Plumsail.DataSource.Dynamics365;
using Plumsail.DataSource.Dynamics365.Settings;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Plumsail.DataSource
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


            builder.Services.Configure<AppSettings>(configuration);
            builder.Services.AddTransient<HttpClientProvider>();

        }
    }
}
