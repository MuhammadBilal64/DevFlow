using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.
                HasOne(p => p.Project)
                .WithMany(m => m.Members)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);

            builder
    .HasOne(pm => pm.User)
    .WithMany(u => u.ProjectMemberships)
    .HasForeignKey(pm => pm.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => new
            {
                p.ProjectId,
                p.UserId,

            }).IsUnique();
            builder.Property(p => p.JoinedAt).IsRequired();
            builder
    .Property(p => p.Role)
    .IsRequired();
        }
    }
}
