namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.DirectoryGroups;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProvisionBusinessAppActiveDirectoryGroupConfiguration : IEntityTypeConfiguration<ProvisionBusinessAppActiveDirectoryGroup>
{
    public void Configure(EntityTypeBuilder<ProvisionBusinessAppActiveDirectoryGroup> builder)
    {
        builder.ToTable("ProvisionBusinessAppActiveDirectoryGroups");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
