namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// AdminAuditLog Configuration
public class AdminAuditLogConfiguration : IEntityTypeConfiguration<AdminAuditLog>
{
    public void Configure(EntityTypeBuilder<AdminAuditLog> builder)
    {
        builder.HasKey(aal => aal.Id);
        builder.Property(aal => aal.Action).IsRequired().HasMaxLength(255);
        builder.Property(aal => aal.EntityType).IsRequired().HasMaxLength(255);
        builder.Property(aal => aal.ActionBy).IsRequired().HasMaxLength(255);
        builder.Property(aal => aal.Description).HasMaxLength(1000);
        // OldValues and NewValues can be large, consider JSONB in PostgreSQL if frequent querying on content is needed.
        // For simple storage, TEXT is sufficient.
        builder.Property(aal => aal.OldValues).HasColumnType("text");
        builder.Property(aal => aal.NewValues).HasColumnType("text");
    }
}
