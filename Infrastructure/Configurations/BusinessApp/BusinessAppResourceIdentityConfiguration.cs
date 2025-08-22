namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppResourceIdentityConfiguration : IEntityTypeConfiguration<BusinessAppResourceIdentity>
{
    public void Configure(EntityTypeBuilder<BusinessAppResourceIdentity> builder)
    {
        builder.ToTable("BusinessAppResourceIdentities", t => t.HasCheckConstraint(
                "ck_business_resource_identity_primary_secondary_distinct",
                @"(""SecondaryOwnerId"" IS NULL) 
                  OR (""SecondaryOwnerId"" <> ""PrimaryOwnerId"")"));

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.BusinessApplication)
            .WithMany(businessApp => businessApp.BusinessAppResourceIdentities)
            .HasForeignKey(businessAppResourceIdentity => businessAppResourceIdentity.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.BusinessAppEnvironment)
            .WithMany(businessAppEnv => businessAppEnv.BusinessAppResourceIdentities)
            .HasForeignKey(businessAppResourceIdentity => new { businessAppResourceIdentity.BusinessApplicationId, businessAppResourceIdentity.BusinessAppEnvironmentId })
            .HasPrincipalKey(e => new { e.BusinessApplicationId, e.Id })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.PrimaryOwner)
            .WithMany(businessAppUser => businessAppUser.BusinessAppResourceIdentitiesPrimaryOwner)
            .HasForeignKey(businessAppResourceIdentity => new { businessAppResourceIdentity.BusinessApplicationId, businessAppResourceIdentity.PrimaryOwnerId })
            .HasPrincipalKey(u => new { u.BusinessApplicationId, u.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppResourceIdentity => businessAppResourceIdentity.SecondaryOwner)
            .WithMany(businessAppUser => businessAppUser.BusinessAppResourceIdentitiesSecondaryOwner)
            .HasForeignKey(businessAppResourceIdentity => new { businessAppResourceIdentity.BusinessApplicationId, businessAppResourceIdentity.SecondaryOwnerId })
            .HasPrincipalKey(u => new { u.BusinessApplicationId, u.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
