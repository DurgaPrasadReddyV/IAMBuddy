namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AddBusinessAppServiceActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<AddBusinessAppServiceActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<AddBusinessAppServiceActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("AddBusinessAppServiceActiveDirectoryGroupMemberships");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
