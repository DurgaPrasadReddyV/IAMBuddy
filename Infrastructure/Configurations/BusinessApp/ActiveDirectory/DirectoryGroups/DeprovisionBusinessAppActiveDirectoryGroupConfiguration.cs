namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.DirectoryGroups;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DeprovisionBusinessAppActiveDirectoryGroupConfiguration : IEntityTypeConfiguration<DeprovisionBusinessAppActiveDirectoryGroup>
{
    public void Configure(EntityTypeBuilder<DeprovisionBusinessAppActiveDirectoryGroup> builder)
    {
        builder.ToTable("DeprovisionBusinessAppActiveDirectoryGroups");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
