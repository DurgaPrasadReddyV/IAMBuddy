namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Database.Roles;

using IAMBuddy.Domain.BusinessApp.MSSQL.Database.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLDatabaseADServiceUserRoleConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLDatabaseADServiceUserRole>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLDatabaseADServiceUserRole> builder)
    {
        builder.ToTable("BusinessAppMSSQLDatabaseADServiceUserRoles");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
