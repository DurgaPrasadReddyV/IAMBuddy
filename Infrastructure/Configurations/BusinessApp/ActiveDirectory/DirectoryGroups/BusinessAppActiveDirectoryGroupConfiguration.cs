namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.DirectoryGroups;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.DirectoryGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryGroupConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryGroup>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryGroup> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryGroups");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(x => x.Description)
            .HasMaxLength(512);
        builder.Property(x => x.SamAccountName)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.UserPrincipalName)
            .HasMaxLength(128);
        builder.Property(x => x.DistinguishedName)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.Sid)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasOne(x => x.AuthoritativeSource)
            .WithMany()
            .HasForeignKey(x => x.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BusinessApplication)
            .WithMany()
            .HasForeignKey(x => x.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BusinessAppResourceIdentity)
            .WithMany()
            .HasForeignKey(x => x.BusinessAppResourceIdentityId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Domain)
            .WithMany()
            .HasForeignKey(x => x.DomainId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrganizationalUnit)
            .WithMany()
            .HasForeignKey(x => x.OrganizationalUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
