namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// SqlServerInstance Configuration
public class SqlServerInstanceConfiguration : IEntityTypeConfiguration<SqlServerInstance>
{
    public void Configure(EntityTypeBuilder<SqlServerInstance> builder)
    {
        builder.HasKey(ssi => ssi.Id);
        builder.Property(ssi => ssi.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(ssi => new { ssi.ServerId, ssi.Name }).IsUnique(); // Instance name unique per server
        builder.Property(ssi => ssi.Port).IsRequired().HasMaxLength(10);
        builder.Property(ssi => ssi.ServiceName).IsRequired().HasMaxLength(255);
        builder.Property(ssi => ssi.ServiceAccount).IsRequired().HasMaxLength(255);
        builder.Property(ssi => ssi.Collation).IsRequired().HasMaxLength(100);
        builder.Property(ssi => ssi.Description).HasMaxLength(500);

        builder.HasOne(ssi => ssi.Server)
               .WithMany(ss => ss.Instances)
               .HasForeignKey(ssi => ssi.ServerId)
               .OnDelete(DeleteBehavior.Restrict); // Don't delete server if instances exist

        builder.HasOne(ssi => ssi.Listener)
               .WithMany(ssl => ssl.Instances)
               .HasForeignKey(ssi => ssi.ListenerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ssi => ssi.Databases)
               .WithOne(db => db.Instance)
               .HasForeignKey(db => db.InstanceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
