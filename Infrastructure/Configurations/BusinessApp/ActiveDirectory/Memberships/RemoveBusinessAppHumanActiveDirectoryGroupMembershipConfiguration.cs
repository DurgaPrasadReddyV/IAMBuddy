namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RemoveBusinessAppHumanActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<RemoveBusinessAppHumanActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<RemoveBusinessAppHumanActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("RemoveBusinessAppHumanActiveDirectoryGroupMemberships");
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
