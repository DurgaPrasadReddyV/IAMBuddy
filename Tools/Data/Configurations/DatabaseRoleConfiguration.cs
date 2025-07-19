namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// DatabaseRole Configuration
public class DatabaseRoleConfiguration : IEntityTypeConfiguration<DatabaseRole>
{
    public void Configure(EntityTypeBuilder<DatabaseRole> builder)
    {
        builder.HasKey(dr => dr.Id);
        builder.Property(dr => dr.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(dr => new { dr.DatabaseId, dr.Name }).IsUnique(); // Role name unique per database
        builder.Property(dr => dr.Description).HasMaxLength(500);

        builder.HasOne(dr => dr.Database)
               .WithMany(db => db.DatabaseRoles)
               .HasForeignKey(dr => dr.DatabaseId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(dr => dr.DatabaseUserRoles)
               .WithOne(dur => dur.DatabaseRole)
               .HasForeignKey(dur => dur.DatabaseRoleId)
               .OnDelete(DeleteBehavior.Restrict); // Don't delete role if users are assigned to it

        builder.HasMany(dr => dr.Permissions)
               .WithOne(p => p.DatabaseRole)
               .HasForeignKey(p => p.DatabaseRoleId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
