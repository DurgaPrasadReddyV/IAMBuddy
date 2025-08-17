namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppEnvironmentConfiguration : IEntityTypeConfiguration<BusinessAppEnvironment>
{
    public void Configure(EntityTypeBuilder<BusinessAppEnvironment> builder)
    {
        builder.ToTable("BusinessAppEnvironments");

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppEnv => businessAppEnv.BusinessApplication)
            .WithMany(businessApp => businessApp.BusinessAppEnvironments)
            .HasForeignKey(businessAppEnv => businessAppEnv.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
