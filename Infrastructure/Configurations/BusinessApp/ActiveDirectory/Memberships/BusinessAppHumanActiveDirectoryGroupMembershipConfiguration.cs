namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppHumanActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<BusinessAppHumanActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<BusinessAppHumanActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("BusinessAppHumanActiveDirectoryGroupMemberships");
        builder.HasOne(x => x.Group)
            .WithMany()
            .HasForeignKey("GroupId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BusinessAppHumanActiveDirectoryAccount)
            .WithMany()
            .HasForeignKey("BusinessAppHumanActiveDirectoryAccountId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
