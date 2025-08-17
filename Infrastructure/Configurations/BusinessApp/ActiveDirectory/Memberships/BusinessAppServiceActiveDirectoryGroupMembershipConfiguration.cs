namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppServiceActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<BusinessAppServiceActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<BusinessAppServiceActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("BusinessAppServiceActiveDirectoryGroupMemberships");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
