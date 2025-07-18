namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PermissionName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.SecurableType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.SecurableName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.GrantedBy)
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.Database)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.DatabaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DatabaseUser)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.DatabaseUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DatabaseRole)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.DatabaseRoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ServerRole)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.ServerRoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
