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
    }
}