namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AddBusinessAppHumanActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<AddBusinessAppHumanActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<AddBusinessAppHumanActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("AddBusinessAppHumanActiveDirectoryGroupMemberships");
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
