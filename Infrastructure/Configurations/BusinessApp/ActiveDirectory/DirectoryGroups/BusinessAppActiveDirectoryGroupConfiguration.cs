namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.DirectoryGroups;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryGroupConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryGroup>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryGroup> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryGroups");

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppADGroup => businessAppADGroup.AuthoritativeSource)
            .WithMany(source => source.BusinessAppActiveDirectoryGroups)
            .HasForeignKey(businessAppADGroup => businessAppADGroup.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppADGroup => businessAppADGroup.BusinessAppResourceIdentity)
            .WithMany(source => source.BusinessAppActiveDirectoryGroups)
            .HasForeignKey(businessAppADGroup => businessAppADGroup.BusinessAppResourceIdentityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
