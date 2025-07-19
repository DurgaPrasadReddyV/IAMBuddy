namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// BusinessApplicationTeamMember Configuration
public class BusinessApplicationTeamMemberConfiguration : IEntityTypeConfiguration<BusinessApplicationTeamMember>
{
    public void Configure(EntityTypeBuilder<BusinessApplicationTeamMember> builder)
    {
        builder.HasKey(batm => batm.Id);
        builder.Property(batm => batm.Role).IsRequired().HasMaxLength(100);

        builder.HasOne(batm => batm.BusinessApplication)
               .WithMany(ba => ba.TeamMembers)
               .HasForeignKey(batm => batm.BusinessApplicationId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(batm => batm.HumanIdentity)
               .WithMany(hi => hi.BusinessApplicationMemberships)
               .HasForeignKey(batm => batm.HumanIdentityId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a human identity if they are a team member
    }
}
