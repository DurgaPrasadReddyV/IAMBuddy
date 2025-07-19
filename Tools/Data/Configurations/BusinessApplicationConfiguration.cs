namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// BusinessApplication Configuration
public class BusinessApplicationConfiguration : IEntityTypeConfiguration<BusinessApplication>
{
    public void Configure(EntityTypeBuilder<BusinessApplication> builder)
    {
        builder.HasKey(ba => ba.Id);
        builder.Property(ba => ba.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(ba => ba.Name).IsUnique();
        builder.Property(ba => ba.ShortName).HasMaxLength(100);
        builder.Property(ba => ba.Description).HasMaxLength(1000);
        builder.Property(ba => ba.BusinessPurpose).HasMaxLength(1000);
        builder.Property(ba => ba.TechnicalContact).HasMaxLength(255);
        builder.Property(ba => ba.BusinessContact).HasMaxLength(255);
        builder.Property(ba => ba.VendorName).HasMaxLength(255);
        builder.Property(ba => ba.Version).HasMaxLength(50);
        builder.Property(ba => ba.AnnualCost).HasColumnType("decimal(18,2)");
        builder.Property(ba => ba.ComplianceRequirements).HasMaxLength(1000);
        builder.Property(ba => ba.DataClassification).HasMaxLength(100);
        builder.Property(ba => ba.SourceCodeRepository).HasMaxLength(500);
        builder.Property(ba => ba.DocumentationLink).HasMaxLength(500);

        // Relationships
        builder.HasOne(ba => ba.ApplicationOwner)
               .WithMany(hi => hi.ApplicationOwned)
               .HasForeignKey(ba => ba.ApplicationOwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ba => ba.AlternateApplicationOwner)
               .WithMany(hi => hi.ApplicationAlternateOwned)
               .HasForeignKey(ba => ba.AlternateApplicationOwnerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ba => ba.ProductOwner)
               .WithMany(hi => hi.ProductOwned)
               .HasForeignKey(ba => ba.ProductOwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ba => ba.AlternateProductOwner)
               .WithMany(hi => hi.ProductAlternateOwned)
               .HasForeignKey(ba => ba.AlternateProductOwnerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ba => ba.Environments)
               .WithOne(bae => bae.BusinessApplication)
               .HasForeignKey(bae => bae.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ba => ba.TeamMembers)
               .WithOne(batm => batm.BusinessApplication)
               .HasForeignKey(batm => batm.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ba => ba.NonHumanIdentities)
               .WithOne(nhi => nhi.BusinessApplication)
               .HasForeignKey(nhi => nhi.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ba => ba.Resources)
               .WithOne(bar => bar.BusinessApplication)
               .HasForeignKey(bar => bar.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
