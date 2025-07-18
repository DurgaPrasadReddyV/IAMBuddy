namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Entity Configurations
public class SqlServerListenerConfiguration : IEntityTypeConfiguration<SqlServerListener>
{
    public void Configure(EntityTypeBuilder<SqlServerListener> builder)
    {
        builder.ToTable("SqlServerListeners");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.IPAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(x => x.Protocol)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.Name, x.IPAddress, x.Port })
            .IsUnique();
    }
}
