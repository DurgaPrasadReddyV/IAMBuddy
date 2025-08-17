namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryOrganizationalUnitConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryOrganizationalUnit>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryOrganizationalUnit> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryOrganizationalUnits");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
