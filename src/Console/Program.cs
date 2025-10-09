using Lucy.Console.Commands.Delete;
using Lucy.Console.Commands.List;
using Lucy.Console.Commands.New;
using Lucy.Console.Commands.Show;
using Lucy.Console.Commands.Update;
using Lucy.Console.Extensions;
using Lucy.Console.Internal;
using Lucy.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Spectre.Console.Cli;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables(prefix: "LUCY_")
    .Build();

services
    .AddLocalization(options => options.ResourcesPath = "Resources")
    .AddInfrastructure(configuration)
    .AddCommands();

// Build the service provider
//  Dispose it when done to clean up any resources
using var provider = services.BuildServiceProvider();

var app = new CommandApp();
var executor = provider.GetRequiredService<ICommandExecutor>();
var localizer = provider.GetRequiredService<IStringLocalizer<Program>>();

app.Configure(config =>
{
    config.SetApplicationName(localizer["App.Name"]);
    config.SetApplicationVersion(
        app.GetVersionFromAssembly(typeof(Program).Assembly));

    #region Command Configuration
    #region New Command Branch

    config.AddBranch<NewCommand>(localizer["Command.New"],
        branch =>
        {
            branch.SetDescription(localizer["Command.New.Description"]);

            branch.AddAsyncDelegate<NewProjectCommand>("New.Project", executor, localizer);
        });

    #endregion
    #region List Command Branch

    config.AddBranch<ListCommand>(localizer["Command.List"],
        branch =>
        {
            branch.SetDescription(localizer["Command.List.Description"]);

            branch.AddAsyncDelegate<ListProjectsCommand>("List.Projects", executor, localizer);
        });

    #endregion
    #region Show Command Branch

    config.AddBranch<ShowCommand>(localizer["Command.Show"], branch =>
    {
        branch.SetDescription(localizer["Command.Show.Description"]);

        branch.AddAsyncDelegate<ShowProjectCommand>("Show.Project", executor, localizer);
    });

    #endregion
    #region Update Command Branch

    config.AddBranch<UpdateCommand>(localizer["Command.Update"], branch =>
    {
        branch.SetDescription(localizer["Command.Update.Description"]);

        branch.AddAsyncDelegate<UpdateProjectCommand>("Update.Project", executor, localizer);
    });

    #endregion
    #region Delete Command Branch

    config.AddBranch<DeleteCommand>(localizer["Command.Delete"], branch =>
    {
        branch.SetDescription(localizer["Command.Delete.Description"]);

        branch.AddAsyncDelegate<DeleteProjectCommand>("Delete.Project", executor, localizer);
    });

    #endregion
    #endregion Command Configuration

});

return await app.RunAsync(args);
