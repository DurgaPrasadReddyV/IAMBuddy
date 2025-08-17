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
    }
}
