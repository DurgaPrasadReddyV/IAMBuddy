namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// DatabaseUser Configuration
public class DatabaseUserConfiguration : IEntityTypeConfiguration<DatabaseUser>
{
    public void Configure(EntityTypeBuilder<DatabaseUser> builder)
    {
        builder.HasKey(du => du.Id);
        builder.Property(du => du.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(du => new { du.DatabaseId, du.Name }).IsUnique(); // User name unique per database
        builder.Property(du => du.DefaultSchema).HasMaxLength(100);
        builder.Property(du => du.UserType).IsRequired().HasMaxLength(50);
        builder.Property(du => du.Description).HasMaxLength(500);

        builder.HasOne(du => du.Database)
               .WithMany(db => db.DatabaseUsers)
               .HasForeignKey(du => du.DatabaseId)
               .OnDelete(DeleteBehavior.Restrict); // Don't delete database if users exist

        builder.HasOne(du => du.ServerLogin)
               .WithMany(sl => sl.DatabaseUsers)
               .HasForeignKey(du => du.ServerLoginId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(du => du.DatabaseUserRoles)
               .WithOne(dur => dur.DatabaseUser)
               .HasForeignKey(dur => dur.DatabaseUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(du => du.Permissions)
               .WithOne(p => p.DatabaseUser)
               .HasForeignKey(p => p.DatabaseUserId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
