namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppResourceIdentityConfiguration : IEntityTypeConfiguration<BusinessAppResourceIdentity>
{
    public void Configure(EntityTypeBuilder<BusinessAppResourceIdentity> builder)
    {
        builder.ToTable("BusinessAppResourceIdentities");

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.BusinessApplication)
            .WithMany(businessApp => businessApp.BusinessAppResourceIdentities)
            .HasForeignKey(businessAppResourceIdentity => businessAppResourceIdentity.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.BusinessApplication)
            .WithMany(businessAppEnv => businessAppEnv.BusinessAppResourceIdentities)
            .HasForeignKey(businessAppResourceIdentity => businessAppResourceIdentity.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.PrimaryOwner)
            .WithMany(businessAppUser => businessAppUser.BusinessAppResourceIdentitiesPrimaryOwner)
            .HasForeignKey(businessAppResourceIdentity => businessAppResourceIdentity.PrimaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.SecondaryOwner)
            .WithMany(businessAppUser => businessAppUser.BusinessAppResourceIdentitiesSecondaryOwner)
            .HasForeignKey(businessAppResourceIdentity => businessAppResourceIdentity.SecondaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
