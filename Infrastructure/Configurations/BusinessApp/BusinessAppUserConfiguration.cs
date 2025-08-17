namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppUserConfiguration : IEntityTypeConfiguration<BusinessAppUser>
{
    public void Configure(EntityTypeBuilder<BusinessAppUser> builder)
    {
        builder.ToTable("BusinessAppUsers");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
