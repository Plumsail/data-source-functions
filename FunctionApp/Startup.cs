using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using D365_BC = Plumsail.DataSource.Dynamics365.BusinessCentral;
using D365_CRM = Plumsail.DataSource.Dynamics365.CRM;
using SP = Plumsail.DataSource.SharePoint;

[assembly: FunctionsStartup(typeof(Plumsail.DataSource.FunctionApp.Startup))]
namespace Plumsail.DataSource.FunctionApp
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

            builder.Services.Configure<D365_BC.Settings.AppSettings>(configuration.GetSection("Dynamics365.BusinessCentral"));
            builder.Services.AddTransient<D365_BC.GraphServiceClientProvider>();

            builder.Services.Configure<D365_CRM.Settings.AppSettings>(configuration.GetSection("Dynamics365.CRM"));
            builder.Services.AddTransient<D365_CRM.HttpClientProvider>();

            builder.Services.Configure<SP.Settings.AppSettings>(configuration.GetSection("SharePoint"));
            builder.Services.AddTransient<SP.GraphServiceClientProvider>();
        }
    }
}
