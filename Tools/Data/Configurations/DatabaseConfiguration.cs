namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Database Configuration
public class DatabaseConfiguration : IEntityTypeConfiguration<Database>
{
    public void Configure(EntityTypeBuilder<Database> builder)
    {
        builder.HasKey(db => db.Id);
        builder.Property(db => db.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(db => new { db.InstanceId, db.Name }).IsUnique(); // Database name unique per instance
        builder.Property(db => db.Collation).IsRequired().HasMaxLength(100);
        builder.Property(db => db.RecoveryModel).IsRequired().HasMaxLength(50);
        builder.Property(db => db.CompatibilityLevel).IsRequired().HasMaxLength(50);
        builder.Property(db => db.Description).HasMaxLength(500);

        builder.HasOne(db => db.Instance)
               .WithMany(ssi => ssi.Databases)
               .HasForeignKey(db => db.InstanceId)
               .OnDelete(DeleteBehavior.Restrict); // Don't delete instance if databases exist

        builder.HasMany(db => db.DatabaseUsers)
               .WithOne(du => du.Database)
               .HasForeignKey(du => du.DatabaseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(db => db.DatabaseRoles)
               .WithOne(dr => dr.Database)
               .HasForeignKey(dr => dr.DatabaseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(db => db.Permissions)
               .WithOne(p => p.Database)
               .HasForeignKey(p => p.DatabaseId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
