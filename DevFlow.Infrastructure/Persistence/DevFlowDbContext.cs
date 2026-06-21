using Microsoft.EntityFrameworkCore;
using DevFlow.Domain.Entities;

namespace DevFlow.Infrastructure.Persistence
{
    public class DevFlowDbContext : DbContext
    {
        public DevFlowDbContext(DbContextOptions<DevFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceMember> WorkspacesMembers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            ConfigureUser(modelBuilder);
            ConfigureRefreshToken(modelBuilder);
            ConfigureWorkspace(modelBuilder);
            ConfigureWorkspaceMember(modelBuilder);

        }
        protected  void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
        protected  void ConfigureRefreshToken(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasOne(u => u.User)
                .WithMany(um => um.RefreshTokens)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<RefreshToken>().HasIndex(rt => rt.Token);

        }
        protected  void ConfigureWorkspace(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workspace>()
                .HasOne(w => w.Creator)
                .WithMany(u => u.CreatedWorkspaces)
                .HasForeignKey(w=>w.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Workspace>()
                .HasMany(u => u.Members)
                .WithOne(ws => ws.Workspace)
                .HasForeignKey(u => u.WorkspaceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Workspace>()
                            .Property(x => x.Name)
                            .HasMaxLength(100);
            modelBuilder.Entity<Workspace>().HasIndex(ws => ws.CreatedBy);

        }
        protected  void ConfigureWorkspaceMember(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkspaceMember>()
                .HasOne(um => um.User)
                .WithMany(u => u.WorkspaceMemberships)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WorkspaceMember>().HasIndex(wsp => new { wsp.UserId, wsp.WorkspaceId }).IsUnique();
        }
    }
}