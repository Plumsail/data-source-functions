using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using D365_BC = Plumsail.DataSource.Dynamics365.BusinessCentral;
using D365_CRM = Plumsail.DataSource.Dynamics365.CRM;
using SP = Plumsail.DataSource.SharePoint;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, builder) => {
        builder.SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) => {
        var configuration = context.Configuration;

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.Configure<D365_BC.Settings.AppSettings>(configuration.GetSection("Dynamics365.BusinessCentral"));
        services.AddTransient<D365_BC.GraphServiceClientProvider>();

        services.Configure<D365_CRM.Settings.AppSettings>(configuration.GetSection("Dynamics365.CRM"));
        services.AddTransient<D365_CRM.HttpClientProvider>();

        services.Configure<SP.Settings.AppSettings>(configuration.GetSection("SharePoint"));
        services.AddTransient<SP.GraphServiceClientProvider>();
    })
    .Build();

host.Run();