namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryDirectoryDomainConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryDirectoryDomain>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryDirectoryDomain> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryDirectoryDomains");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
