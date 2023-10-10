using CleanArchitectureTemplate.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Configurations;

public class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.Property(s => s.TableName)
            .HasMaxLength(50);
        builder.Property(s => s.PrimaryKeyName)
            .HasMaxLength(50);
        builder.Property(u => u.CreatedDate)
            .HasDefaultValueSql("NOW()");

        #region Relationships
        builder.HasOne(u => u.User)
            .WithMany(c => c.AuditLogs)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        #endregion
    }
}
