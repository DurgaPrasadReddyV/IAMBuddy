namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Database.Roles;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLDatabasePermissionConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLDatabasePermission>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLDatabasePermission> builder)
    {
        builder.ToTable("BusinessAppMSSQLDatabasePermissions");
        builder.HasOne(x => x.DatabaseRole)
            .WithMany()
            .HasForeignKey("DatabaseRoleId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
