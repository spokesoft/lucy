using Lucy.Application.Interfaces;
using Lucy.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables(prefix: "LUCY_")
    .Build();

services.AddInfrastructure(configuration);

var provider = services.BuildServiceProvider();

var migrators = provider.GetServices<IDatabaseMigrator>();

await Task.WhenAll(migrators.Select(m => m.MigrateAsync()));
