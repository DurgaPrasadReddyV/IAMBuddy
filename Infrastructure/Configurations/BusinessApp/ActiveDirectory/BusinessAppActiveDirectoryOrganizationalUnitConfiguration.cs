namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryOrganizationalUnitConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryOrganizationalUnit>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryOrganizationalUnit> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryOrganizationalUnits");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.DistinguishedName).HasMaxLength(256);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
