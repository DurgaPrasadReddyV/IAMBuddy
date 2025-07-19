namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ActiveDirectoryGroup Configuration
public class ActiveDirectoryGroupConfiguration : IEntityTypeConfiguration<ActiveDirectoryGroup>
{
    public void Configure(EntityTypeBuilder<ActiveDirectoryGroup> builder)
    {
        builder.HasKey(adg => adg.Id);
        builder.Property(adg => adg.Name).IsRequired().HasMaxLength(255);
        builder.Property(adg => adg.SamAccountName).IsRequired().HasMaxLength(255);
        builder.HasIndex(adg => adg.SamAccountName).IsUnique();
        builder.Property(adg => adg.DisplayName).HasMaxLength(255);
        builder.Property(adg => adg.Description).HasMaxLength(1000);
        builder.Property(adg => adg.Domain).IsRequired().HasMaxLength(255);
        builder.Property(adg => adg.DistinguishedName).IsRequired().HasMaxLength(1000);
        builder.HasIndex(adg => adg.DistinguishedName).IsUnique();
        builder.Property(adg => adg.Email).HasMaxLength(255);
        builder.Property(adg => adg.ManagedBy).HasMaxLength(255);

        builder.HasOne(adg => adg.NonHumanIdentity)
               .WithMany() // No direct navigation from NonHumanIdentity to ActiveDirectoryGroup
               .HasForeignKey(adg => adg.NonHumanIdentityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(adg => adg.GroupMemberships)
               .WithOne(adgm => adgm.Group)
               .HasForeignKey(adgm => adgm.GroupId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(adg => adg.ServerLogins)
               .WithOne(sl => sl.ActiveDirectoryGroup)
               .HasForeignKey(sl => sl.ActiveDirectoryGroupId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
