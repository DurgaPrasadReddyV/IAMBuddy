namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.DirectoryGroups;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UpdateBusinessAppActiveDirectoryGroupConfiguration : IEntityTypeConfiguration<UpdateBusinessAppActiveDirectoryGroup>
{
    public void Configure(EntityTypeBuilder<UpdateBusinessAppActiveDirectoryGroup> builder)
    {
        builder.ToTable("UpdateBusinessAppActiveDirectoryGroups");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
