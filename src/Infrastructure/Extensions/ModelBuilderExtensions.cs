using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Extensions;

/// <summary>
/// Extension methods for ModelBuilder.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies all entity configurations from the specified assembly and namespace.
    /// </summary>
    public static void ApplyConfigurationsFromNamespace(this ModelBuilder modelBuilder, Assembly assembly, string @namespace)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            assembly,
            type => type.Namespace == @namespace);
    }
}
