namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ActiveDirectoryGroupMembership Configuration
public class ActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<ActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<ActiveDirectoryGroupMembership> builder)
    {
        builder.HasKey(adgm => adgm.Id);
        builder.HasIndex(adgm => new { adgm.GroupId, adgm.AccountId, adgm.ChildGroupId }).IsUnique(); // Ensure unique membership

        builder.HasOne(adgm => adgm.Group)
               .WithMany(adg => adg.GroupMemberships)
               .HasForeignKey(adgm => adgm.GroupId)
               .OnDelete(DeleteBehavior.Restrict); // Group should not be deleted if it has members

        builder.HasOne(adgm => adgm.Account)
               .WithMany(ada => ada.GroupMemberships)
               .HasForeignKey(adgm => adgm.AccountId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade); // If an account is deleted, remove its group memberships

        builder.HasOne(adgm => adgm.ChildGroup)
               .WithMany() // No direct navigation from ActiveDirectoryGroup to its child group memberships
               .HasForeignKey(adgm => adgm.ChildGroupId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a group if it's a member of another group
    }
}
