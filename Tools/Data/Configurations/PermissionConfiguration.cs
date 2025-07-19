namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Permission Configuration
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PermissionName).IsRequired().HasMaxLength(255);
        builder.Property(p => p.SecurableName).IsRequired().HasMaxLength(255);
        builder.Property(p => p.GrantedBy).HasMaxLength(255);
        builder.Property(p => p.Description).HasMaxLength(1000);

        builder.HasOne(p => p.Database)
               .WithMany(db => db.Permissions)
               .HasForeignKey(p => p.DatabaseId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict); // Restrict to avoid unintended cascade deletion of DB

        builder.HasOne(p => p.DatabaseUser)
               .WithMany(du => du.Permissions)
               .HasForeignKey(p => p.DatabaseUserId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.DatabaseRole)
               .WithMany(dr => dr.Permissions)
               .HasForeignKey(p => p.DatabaseRoleId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ServerRole)
               .WithMany(sr => sr.Permissions)
               .HasForeignKey(p => p.ServerRoleId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
