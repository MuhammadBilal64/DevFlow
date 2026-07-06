using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class WorkflowConditionConfiguration : IEntityTypeConfiguration<WorkflowCondition>
    {
        public void Configure(EntityTypeBuilder<WorkflowCondition> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Field)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.Operator)
                .IsRequired();

            builder.HasOne(x => x.Workflow)
                .WithMany(x => x.Conditions)
                .HasForeignKey(x => x.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}