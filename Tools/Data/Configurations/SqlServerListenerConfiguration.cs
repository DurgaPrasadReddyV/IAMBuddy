namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// SqlServerListener Configuration
public class SqlServerListenerConfiguration : IEntityTypeConfiguration<SqlServerListener>
{
    public void Configure(EntityTypeBuilder<SqlServerListener> builder)
    {
        builder.HasKey(sl => sl.Id);
        builder.Property(sl => sl.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(sl => sl.Name).IsUnique();
        builder.Property(sl => sl.IPAddress).IsRequired().HasMaxLength(50);
        builder.Property(sl => sl.Protocol).IsRequired().HasMaxLength(50);
        builder.Property(sl => sl.Description).HasMaxLength(500);

        builder.HasMany(sl => sl.Instances)
               .WithOne(ssi => ssi.Listener)
               .HasForeignKey(ssi => ssi.ListenerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
