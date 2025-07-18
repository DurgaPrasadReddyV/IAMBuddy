namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatabaseUserRoleConfiguration : IEntityTypeConfiguration<DatabaseUserRole>
{
    public void Configure(EntityTypeBuilder<DatabaseUserRole> builder)
    {
        builder.ToTable("DatabaseUserRoles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssignedBy)
            .HasMaxLength(128);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.DatabaseUser)
            .WithMany(x => x.DatabaseUserRoles)
            .HasForeignKey(x => x.DatabaseUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DatabaseRole)
            .WithMany(x => x.DatabaseUserRoles)
            .HasForeignKey(x => x.DatabaseRoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.DatabaseUserId, x.DatabaseRoleId })
            .IsUnique();
    }
}
