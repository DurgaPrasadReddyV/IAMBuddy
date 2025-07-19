namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ServerRole Configuration
public class ServerRoleConfiguration : IEntityTypeConfiguration<ServerRole>
{
    public void Configure(EntityTypeBuilder<ServerRole> builder)
    {
        builder.HasKey(sr => sr.Id);
        builder.Property(sr => sr.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(sr => new { sr.ServerId, sr.Name }).IsUnique(); // Role name unique per server
        builder.Property(sr => sr.Description).HasMaxLength(500);

        builder.HasOne(sr => sr.Server)
               .WithMany(ss => ss.ServerRoles)
               .HasForeignKey(sr => sr.ServerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sr => sr.ServerLoginRoles)
               .WithOne(slr => slr.ServerRole)
               .HasForeignKey(slr => slr.ServerRoleId)
               .OnDelete(DeleteBehavior.Restrict); // Don't delete role if logins are assigned to it

        builder.HasMany(sr => sr.Permissions)
               .WithOne(p => p.ServerRole)
               .HasForeignKey(p => p.ServerRoleId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
