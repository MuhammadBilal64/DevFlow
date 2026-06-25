using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> modelBuilder)
        {
            modelBuilder
              .HasOne(u => u.Workspace)
              .WithMany(t => t.Projects)
              .HasForeignKey(p => p.WorkspaceId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder
                    .HasOne(p => p.Creator)
                    .WithMany(t => t.CreatedProjects)
                    .HasForeignKey(p => p.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            modelBuilder
                .HasIndex(p => new { p.WorkspaceId, p.Name }).IsUnique();
            modelBuilder
                            .Property(p => p.Name)
                            .HasMaxLength(100);
        }
    }
}
