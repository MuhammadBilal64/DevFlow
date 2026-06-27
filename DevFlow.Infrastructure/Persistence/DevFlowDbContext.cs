using Microsoft.EntityFrameworkCore;
using DevFlow.Domain.Entities;
using System.Security.Cryptography;

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
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevFlowDbContext).Assembly);
           
          

        }
      
     
       
    
       
    }
}