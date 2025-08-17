namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Memberships;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppServiceActiveDirectoryGroupMembershipConfiguration : IEntityTypeConfiguration<BusinessAppServiceActiveDirectoryGroupMembership>
{
    public void Configure(EntityTypeBuilder<BusinessAppServiceActiveDirectoryGroupMembership> builder)
    {
        builder.ToTable("BusinessAppServiceActiveDirectoryGroupMemberships");
        builder.HasOne(x => x.Group)
            .WithMany()
            .HasForeignKey("GroupId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BusinessAppServiceActiveDirectoryAccount)
            .WithMany()
            .HasForeignKey("BusinessAppServiceActiveDirectoryAccountId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
