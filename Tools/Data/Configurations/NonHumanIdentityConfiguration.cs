namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// NonHumanIdentity Configuration
public class NonHumanIdentityConfiguration : IEntityTypeConfiguration<NonHumanIdentity>
{
    public void Configure(EntityTypeBuilder<NonHumanIdentity> builder)
    {
        builder.HasKey(nhi => nhi.Id);
        builder.Property(nhi => nhi.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(nhi => nhi.Name).IsUnique(); // Non-human identities should generally have unique names
        builder.Property(nhi => nhi.DisplayName).HasMaxLength(255);
        builder.Property(nhi => nhi.Purpose).HasMaxLength(1000);
        builder.Property(nhi => nhi.TechnicalContact).HasMaxLength(255);
        builder.Property(nhi => nhi.AccessFrequency).HasMaxLength(50);
        builder.Property(nhi => nhi.Description).HasMaxLength(1000);

        builder.HasOne(nhi => nhi.BusinessApplication)
               .WithMany(ba => ba.NonHumanIdentities)
               .HasForeignKey(nhi => nhi.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(nhi => nhi.BusinessApplicationEnvironment)
               .WithMany(bae => bae.NonHumanIdentities)
               .HasForeignKey(nhi => nhi.BusinessApplicationEnvironmentId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(nhi => nhi.PrimaryOwner)
               .WithMany(hi => hi.PrimaryOwnedNonHumanIdentities)
               .HasForeignKey(nhi => nhi.PrimaryOwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(nhi => nhi.AlternateOwner)
               .WithMany(hi => hi.AlternateOwnedNonHumanIdentities)
               .HasForeignKey(nhi => nhi.AlternateOwnerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(nhi => nhi.ActiveDirectoryAccounts)
               .WithOne(ada => ada.NonHumanIdentity)
               .HasForeignKey(ada => ada.NonHumanIdentityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(nhi => nhi.ServerLogins)
               .WithOne(sl => sl.NonHumanIdentity)
               .HasForeignKey(sl => sl.NonHumanIdentityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
