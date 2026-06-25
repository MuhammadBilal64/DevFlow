using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder
                .HasOne(p => p.Project)
                .WithMany(t => t.Tasks)
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(i => i.Creator)
                .WithMany(y => y.CreatedTasks)
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(p => p.Assignee)
                .WithMany(p => p.AssignedTasks)
                .HasForeignKey(s => s.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
