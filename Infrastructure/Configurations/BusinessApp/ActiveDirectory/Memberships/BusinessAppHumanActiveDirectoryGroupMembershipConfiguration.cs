namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppHumanActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<BusinessAppHumanActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<BusinessAppHumanActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("BusinessAppHumanActiveDirectoryGroupMemberships");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
