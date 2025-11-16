using Lucy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the ProjectComment entity.
/// </summary>
public class ProjectCommentTypeConfiguration : IEntityTypeConfiguration<ProjectComment>
{
    public void Configure(EntityTypeBuilder<ProjectComment> builder)
    {
        builder.HasIndex(c => c.ProjectId);
    }
}
