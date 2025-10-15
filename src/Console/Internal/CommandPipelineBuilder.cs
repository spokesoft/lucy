using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Lucy.Console.Internal;

/// <summary>
/// Builds a command pipeline with middleware.
/// </summary>
internal class CommandPipelineBuilder<TCommand>
    where TCommand : CommandSettings
{
    private readonly List<CommandMiddlewareDelegate<TCommand>> _middlewares = [];
    private readonly IServiceProvider _services;

    /// <summary>
    /// Private constructor to initialize the builder with a service provider.
    /// </summary>
    private CommandPipelineBuilder(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Creates a new instance of the command pipeline builder.
    /// </summary>
    public static CommandPipelineBuilder<TCommand> Create(IServiceProvider services) => new(services);

    /// <summary>
    /// Adds a middleware function to the pipeline.
    /// </summary>
    public CommandPipelineBuilder<TCommand> Use(CommandMiddlewareDelegate<TCommand> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    /// <summary>
    /// Adds a middleware to the pipeline by resolving it from the service provider.
    /// </summary>
    public CommandPipelineBuilder<TCommand> Use<TMiddleware>()
    {
        var middleware = _services.GetRequiredService(typeof(TMiddleware))
            as ICommandMiddleware
            ?? throw new InvalidOperationException($"The middleware {typeof(TMiddleware).FullName} is not registered.");

        return Use(middleware);
    }

    /// <summary>
    /// Adds a middleware instance to the pipeline.
    /// </summary>
    public CommandPipelineBuilder<TCommand> Use(ICommandMiddleware middleware)
    {
        _middlewares.Add(middleware.InvokeAsync);
        return this;
    }

    /// <summary>
    /// Builds the command pipeline.
    /// </summary>
    public CommandPipeline<TCommand> Build()
    {
        CommandDelegate<TCommand> app = (context, command, token) =>
        {
            // No terminal middleware, return success.
            return Task.FromResult(ExitCode.Success);
        };

        foreach (var middleware in _middlewares.AsEnumerable().Reverse())
        {
            var next = app;
            app = (context, command, token) => middleware(context, command, next, token);
        }

        return new CommandPipeline<TCommand>(app);
    }
}
