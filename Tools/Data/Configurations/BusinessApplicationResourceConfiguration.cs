namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// BusinessApplicationResource Configuration
public class BusinessApplicationResourceConfiguration : IEntityTypeConfiguration<BusinessApplicationResource>
{
    public void Configure(EntityTypeBuilder<BusinessApplicationResource> builder)
    {
        builder.HasKey(bar => bar.Id);
        builder.Property(bar => bar.ResourceType).IsRequired().HasMaxLength(100);
        builder.Property(bar => bar.ResourceName).HasMaxLength(255);
        builder.Property(bar => bar.Purpose).HasMaxLength(500);

        builder.HasOne(bar => bar.BusinessApplication)
               .WithMany(ba => ba.Resources)
               .HasForeignKey(bar => bar.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
