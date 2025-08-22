namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessApplicationConfiguration : IEntityTypeConfiguration<BusinessApplication>
{
    public void Configure(EntityTypeBuilder<BusinessApplication> builder)
    {
        builder.ToTable("BusinessApplications", t => t.HasCheckConstraint(
                "ck_business_application_primary_secondary_distinct",
                @"(""SecondaryOwnerId"" IS NULL) 
                  OR (""SecondaryOwnerId"" <> ""PrimaryOwnerId"")"));

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessApp => businessApp.BusinessContact)
            .WithOne(businessAppUser => businessAppUser.BusinessAppBusinessContact)
            .HasForeignKey<BusinessApplication>(businessApp => new { businessApp.Id, businessApp.BusinessContactId })
            .HasPrincipalKey((BusinessAppUser u) => new { u.BusinessApplicationId, u.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.TechnicalContact)
            .WithOne(businessAppUser => businessAppUser.BusinessAppTechnicalContact)
            .HasForeignKey<BusinessApplication>(businessApp => new { businessApp.Id, businessApp.TechnicalContactId })
            .HasPrincipalKey((BusinessAppUser u) => new { u.BusinessApplicationId, u.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.PrimaryOwner)
            .WithOne(businessAppUser => businessAppUser.BusinessAppPrimaryOwner)
            .HasForeignKey<BusinessApplication>(businessApp => new { businessApp.Id, businessApp.PrimaryOwnerId })
            .HasPrincipalKey((BusinessAppUser u) => new { u.BusinessApplicationId, u.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.SecondaryOwner)
            .WithOne(businessAppUser => businessAppUser.BusinessAppSecondaryOwner)
            .HasForeignKey<BusinessApplication>(businessApp => new { businessApp.Id, businessApp.SecondaryOwnerId })
            .HasPrincipalKey((BusinessAppUser u) => new { u.BusinessApplicationId, u.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.AuthoritativeSource)
            .WithMany(source => source.BusinessApplications)
            .HasForeignKey(businessApp => businessApp.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
