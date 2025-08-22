namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppUserConfiguration : IEntityTypeConfiguration<BusinessAppUser>
{
    public void Configure(EntityTypeBuilder<BusinessAppUser> builder)
    {
        builder.ToTable("BusinessAppUsers");
        builder.HasKey(x => new { x.BusinessApplicationId, x.HumanIdentityId });

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppUser => businessAppUser.HumanIdentity)
            .WithMany(human => human.BusinessAppUsers)
            .HasForeignKey(businessAppUser => businessAppUser.HumanIdentityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppUser => businessAppUser.BusinessApplication)
            .WithMany(businessApp => businessApp.BusinessAppUsers)
            .HasForeignKey(businessAppUser => businessAppUser.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
