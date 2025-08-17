namespace IAMBuddy.Infrastructure.Configurations.Common;

using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AdminAuditLogConfiguration : IEntityTypeConfiguration<AdminAuditLog>
{
    public void Configure(EntityTypeBuilder<AdminAuditLog> builder)
    {
        builder.ToTable("AdminAuditLogs");

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.EntityId)
            .IsRequired();
    }
}
