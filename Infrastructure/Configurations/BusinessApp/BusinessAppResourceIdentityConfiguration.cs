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
    }
}
