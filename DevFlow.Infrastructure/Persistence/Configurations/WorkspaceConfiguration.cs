using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> modelBuilder)
        {
            modelBuilder
     .HasOne(w => w.Creator)
     .WithMany(u => u.CreatedWorkspaces)
     .HasForeignKey(w => w.CreatedBy)
     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasMany(u => u.Members)
                .WithOne(ws => ws.Workspace)
                .HasForeignKey(u => u.WorkspaceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                            .Property(x => x.Name)
                            .HasMaxLength(100);
            modelBuilder.HasIndex(ws => ws.CreatedBy);
        }
    }
}
