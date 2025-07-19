namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// DatabaseUserRole Configuration
public class DatabaseUserRoleConfiguration : IEntityTypeConfiguration<DatabaseUserRole>
{
    public void Configure(EntityTypeBuilder<DatabaseUserRole> builder)
    {
        builder.HasKey(dur => dur.Id);
        builder.HasIndex(dur => new { dur.DatabaseUserId, dur.DatabaseRoleId }).IsUnique(); // Unique assignment

        builder.HasOne(dur => dur.DatabaseUser)
               .WithMany(du => du.DatabaseUserRoles)
               .HasForeignKey(dur => dur.DatabaseUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dur => dur.DatabaseRole)
               .WithMany(dr => dr.DatabaseUserRoles)
               .HasForeignKey(dur => dur.DatabaseRoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
