namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Server.Roles;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerPermissionConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerPermission>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerPermission> builder)
    {
        builder.ToTable("BusinessAppMSSQLServerPermissions");
        builder.HasOne(x => x.ServerRole)
            .WithMany()
            .HasForeignKey("ServerRoleId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
