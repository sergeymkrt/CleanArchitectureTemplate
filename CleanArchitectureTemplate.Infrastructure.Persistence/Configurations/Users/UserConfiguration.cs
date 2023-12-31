﻿using CleanArchitectureTemplate.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasIndex(c => c.Email)
            .IsUnique();
        
        #region Seed Data
        // Add system user
        builder.HasData(
            new User(
                1,
                "User",
                "System",
                "system.developers@example.com"));
        #endregion
    }
}
