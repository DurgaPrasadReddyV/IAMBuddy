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

        builder.HasOne(businessAppADHumanAccMembership => businessAppADHumanAccMembership.AuthoritativeSource)
            .WithMany(source => source.BusinessAppHumanActiveDirectoryGroupMemberships)
            .HasForeignKey(businessAppADHumanAccMembership => businessAppADHumanAccMembership.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppADHumanAccMembership => businessAppADHumanAccMembership.BusinessAppUser)
            .WithMany(source => source.BusinessAppHumanActiveDirectoryGroupMemberships)
            .HasForeignKey(businessAppADHumanAccMembership => new { businessAppADHumanAccMembership.BusinessApplicationId, businessAppADHumanAccMembership.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
