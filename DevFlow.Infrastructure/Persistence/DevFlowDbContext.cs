using Microsoft.EntityFrameworkCore;
using DevFlow.Domain.Entities;
using System.Security.Cryptography;
using DevFlow.Application.Abstractions;

namespace DevFlow.Infrastructure.Persistence
{
    public class DevFlowDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        public DevFlowDbContext(DbContextOptions<DevFlowDbContext> options,IDomainEventDispatcher domainEventDispatcher)
            : base(options)
        {
            _domainEventDispatcher = domainEventDispatcher;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceMember> WorkspacesMembers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks {  get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevFlowDbContext).Assembly);
           
          

        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //collecting events
            var domainEvents = ChangeTracker.Entries<BaseEntity>().SelectMany(entry => entry.Entity.DomainEvents).ToList();
            //clearing events
            foreach (var entity in ChangeTracker.Entries<BaseEntity>())
            {
                entity.Entity.ClearDomainEvents();
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            //publishing events
            foreach (var domainEvent in domainEvents)
            {
                await _domainEventDispatcher.PublishAsync(domainEvent);

            }

            return result;


        }
    }
}