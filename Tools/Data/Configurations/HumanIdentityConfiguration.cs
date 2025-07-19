namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Example Configuration for HumanIdentity
public class HumanIdentityConfiguration : IEntityTypeConfiguration<HumanIdentity>
{
    public void Configure(EntityTypeBuilder<HumanIdentity> builder)
    {
        builder.HasKey(hi => hi.Id);
        builder.Property(hi => hi.UserId).IsRequired().HasMaxLength(100);
        builder.Property(hi => hi.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(hi => hi.LastName).IsRequired().HasMaxLength(100);
        builder.Property(hi => hi.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(hi => hi.Email).IsUnique(); // Email should be unique
        builder.Property(hi => hi.Phone).HasMaxLength(50);
        builder.Property(hi => hi.JobTitle).HasMaxLength(100);
        builder.Property(hi => hi.Department).HasMaxLength(100);
        builder.Property(hi => hi.Division).HasMaxLength(100);
        builder.Property(hi => hi.CostCenter).HasMaxLength(50);
        builder.Property(hi => hi.Location).HasMaxLength(100);
        builder.Property(hi => hi.Manager).HasMaxLength(100);
        builder.Property(hi => hi.EmployeeId).HasMaxLength(50);
        builder.HasIndex(hi => hi.EmployeeId).IsUnique().HasFilter("\"EmployeeId\" IS NOT NULL"); // Unique if not null
        builder.Property(hi => hi.Company).HasMaxLength(100);
        builder.Property(hi => hi.Description).HasMaxLength(1000);

        // Relationships
        builder.HasMany(hi => hi.ActiveDirectoryAccounts)
               .WithOne(ada => ada.HumanIdentity)
               .HasForeignKey(ada => ada.HumanIdentityId)
               .OnDelete(DeleteBehavior.SetNull); // Set Null for accounts if human identity is deleted

        builder.HasMany(hi => hi.PrimaryOwnedNonHumanIdentities)
               .WithOne(nhi => nhi.PrimaryOwner)
               .HasForeignKey(nhi => nhi.PrimaryOwnerId)
               .OnDelete(DeleteBehavior.Restrict); // Restrict to prevent accidental deletion of owners

        builder.HasMany(hi => hi.AlternateOwnedNonHumanIdentities)
               .WithOne(nhi => nhi.AlternateOwner)
               .HasForeignKey(nhi => nhi.AlternateOwnerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(hi => hi.ApplicationOwned)
               .WithOne(ba => ba.ApplicationOwner)
               .HasForeignKey(ba => ba.ApplicationOwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(hi => hi.ApplicationAlternateOwned)
               .WithOne(ba => ba.AlternateApplicationOwner)
               .HasForeignKey(ba => ba.AlternateApplicationOwnerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(hi => hi.ProductOwned)
               .WithOne(ba => ba.ProductOwner)
               .HasForeignKey(ba => ba.ProductOwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(hi => hi.ProductAlternateOwned)
               .WithOne(ba => ba.AlternateProductOwner)
               .HasForeignKey(ba => ba.AlternateProductOwnerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(hi => hi.BusinessApplicationMemberships)
               .WithOne(batm => batm.HumanIdentity)
               .HasForeignKey(batm => batm.HumanIdentityId)
               .OnDelete(DeleteBehavior.Cascade); // If human identity is removed, their team memberships are also removed
    }
}
