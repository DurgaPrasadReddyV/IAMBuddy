namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ActiveDirectoryAccount Configuration
public class ActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<ActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<ActiveDirectoryAccount> builder)
    {
        builder.HasKey(ada => ada.Id);
        builder.Property(ada => ada.SamAccountName).IsRequired().HasMaxLength(255);
        builder.HasIndex(ada => ada.SamAccountName).IsUnique();
        builder.Property(ada => ada.UserPrincipalName).HasMaxLength(255);
        builder.HasIndex(ada => ada.UserPrincipalName).IsUnique().HasFilter("\"UserPrincipalName\" IS NOT NULL");
        builder.Property(ada => ada.DisplayName).HasMaxLength(255);
        builder.Property(ada => ada.Description).HasMaxLength(1000);
        builder.Property(ada => ada.Domain).IsRequired().HasMaxLength(255);
        builder.Property(ada => ada.DistinguishedName).IsRequired().HasMaxLength(1000);
        builder.HasIndex(ada => ada.DistinguishedName).IsUnique();
        builder.Property(ada => ada.ServicePrincipalNames).HasMaxLength(2000); // Store as JSON string
        builder.Property(ada => ada.ManagedBy).HasMaxLength(255);

        builder.HasOne(ada => ada.HumanIdentity)
               .WithMany(hi => hi.ActiveDirectoryAccounts)
               .HasForeignKey(ada => ada.HumanIdentityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ada => ada.NonHumanIdentity)
               .WithMany(nhi => nhi.ActiveDirectoryAccounts)
               .HasForeignKey(ada => ada.NonHumanIdentityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(ada => ada.GroupMemberships)
               .WithOne(adgm => adgm.Account)
               .HasForeignKey(adgm => adgm.AccountId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ada => ada.ServerLogins)
               .WithOne(sl => sl.ActiveDirectoryAccount)
               .HasForeignKey(sl => sl.ActiveDirectoryAccountId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
