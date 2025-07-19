namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// BusinessApplicationEnvironment Configuration
public class BusinessApplicationEnvironmentConfiguration : IEntityTypeConfiguration<BusinessApplicationEnvironment>
{
    public void Configure(EntityTypeBuilder<BusinessApplicationEnvironment> builder)
    {
        builder.HasKey(bae => bae.Id);
        builder.Property(bae => bae.EnvironmentName).HasMaxLength(100);
        builder.Property(bae => bae.Description).HasMaxLength(500);
        builder.Property(bae => bae.Url).HasMaxLength(500);

        builder.HasOne(bae => bae.BusinessApplication)
               .WithMany(ba => ba.Environments)
               .HasForeignKey(bae => bae.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(bae => bae.NonHumanIdentities)
               .WithOne(nhi => nhi.BusinessApplicationEnvironment)
               .HasForeignKey(nhi => nhi.BusinessApplicationEnvironmentId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
