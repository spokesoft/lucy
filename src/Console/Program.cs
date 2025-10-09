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
using Spectre.Console.Cli;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables(prefix: "LUCY_")
    .Build();

services
    .AddInfrastructure(configuration)
    .AddSingleton<ICommandExecutor, CommandExecutor>();

// Build the service provider
//  Dispose it when done to clean up any resources
using var provider = services.BuildServiceProvider();

var app = new CommandApp();
var pipeline = provider.GetRequiredService<ICommandExecutor>();

app.Configure(config =>
{
    config.SetApplicationName("lucy");
    config.SetApplicationVersion(
        app.GetVersionFromAssembly(typeof(Program).Assembly));

    #region Command Configuration
    #region New Command Branch

    config.AddBranch<NewCommand>("new", branch =>
    {
        branch.SetDescription("Create a new resource");

        branch.AddAsyncDelegate<NewProjectCommand>("project", pipeline.ExecuteAsync)
            .WithDescription("Create a new project")
            .WithExample(["new", "project", "EXAMP"])
            .WithExample(["new", "project", "EXAMP", "--name", "Example"]);
    });

    #endregion
    #region List Command Branch

    config.AddBranch<ListCommand>("list", branch =>
    {
        branch.SetDescription("List available resources");

        branch.AddAsyncDelegate<ListProjectsCommand>("project", pipeline.ExecuteAsync)
            .WithDescription("List projects")
            .WithExample(["list", "projects"]);
    });

    #endregion
    #region Show Command Branch

    config.AddBranch<ShowCommand>("show", branch =>
    {
        branch.SetDescription("Show details of a resource");

        branch.AddAsyncDelegate<ShowProjectCommand>("project", pipeline.ExecuteAsync)
            .WithDescription("Show project details")
            .WithExample(["show", "project", "EXAMP"])
            .WithExample(["show", "project", "--id", "1"]);
    });

    #endregion
    #region Update Command Branch

    config.AddBranch<UpdateCommand>("update", branch =>
    {
        branch.SetDescription("Update a resource");

        branch.AddAsyncDelegate<UpdateProjectCommand>("project", pipeline.ExecuteAsync)
            .WithDescription("Update project details")
            .WithExample(["update", "project", "EXAMP", "--name", "NewName"])
            .WithExample(["update", "project", "--id", "1", "-n", "NewName"])
            .WithExample(["update", "project", "NEWKEY", "--id", "1"]);
    });

    #endregion
    #region Delete Command Branch

    config.AddBranch<DeleteCommand>("delete", branch =>
    {
        branch.SetDescription("Delete a resource");

        branch.AddAsyncDelegate<DeleteProjectCommand>("project", pipeline.ExecuteAsync)
            .WithDescription("Delete a project")
            .WithExample(["delete", "project", "EXAMP"])
            .WithExample(["delete", "project", "--id", "1"]);
    });

    #endregion
    #endregion Command Configuration

});

return await app.RunAsync(args);
