using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using DevFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevFlow.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> modelBuilder)
        {
            modelBuilder.HasIndex(rt => rt.Token).IsUnique();
        }
    }
}
