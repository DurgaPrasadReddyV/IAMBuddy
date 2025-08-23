namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RemoveBusinessAppServiceActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<RemoveBusinessAppServiceActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<RemoveBusinessAppServiceActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("RemoveBusinessAppServiceActiveDirectoryGroupMemberships");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
