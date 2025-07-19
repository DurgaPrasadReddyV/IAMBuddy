namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ServerLoginRole Configuration
public class ServerLoginRoleConfiguration : IEntityTypeConfiguration<ServerLoginRole>
{
    public void Configure(EntityTypeBuilder<ServerLoginRole> builder)
    {
        builder.HasKey(slr => slr.Id);
        builder.HasIndex(slr => new { slr.ServerLoginId, slr.ServerRoleId }).IsUnique(); // Unique assignment

        builder.HasOne(slr => slr.ServerLogin)
               .WithMany(sl => sl.ServerLoginRoles)
               .HasForeignKey(slr => slr.ServerLoginId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(slr => slr.ServerRole)
               .WithMany(sr => sr.ServerLoginRoles)
               .HasForeignKey(slr => slr.ServerRoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
