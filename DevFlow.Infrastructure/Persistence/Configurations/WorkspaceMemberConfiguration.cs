using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
    {
        public void Configure(EntityTypeBuilder<WorkspaceMember> modelBuilder)
        {
            modelBuilder
                .HasOne(um => um.User)
                .WithMany(u => u.WorkspaceMemberships)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasIndex(wsp => new { wsp.UserId, wsp.WorkspaceId }).IsUnique();
        }
    }
}
