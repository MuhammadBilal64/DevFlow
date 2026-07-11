using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class WorkflowActionConfiguration : IEntityTypeConfiguration<WorkflowAction>
    {
        public void Configure(EntityTypeBuilder<WorkflowAction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ActionType)
                .IsRequired();

            builder.Property(x => x.Parameters)
                .IsRequired();

            builder.Property(x => x.Order)
                .IsRequired();

            builder.HasOne(x => x.Workflow)
                .WithMany(x => x.Actions)
                .HasForeignKey(x => x.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.WorkflowId, x.Order })
                .IsUnique();
        }
    }
}