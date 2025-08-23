namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryOrganizationalUnitConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryOrganizationalUnit>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryOrganizationalUnit> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryOrganizationalUnits", t => t.HasCheckConstraint("CK_BusinessAppActiveDirectoryOrganizationalUnits_NoSelfParent",
                "\"ParentOuId\" IS NULL OR \"Id\" <> \"ParentOuId\""));

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppADOU => businessAppADOU.AuthoritativeSource)
           .WithMany(source => source.BusinessAppActiveDirectoryOrganizationalUnits)
           .HasForeignKey(businessAppADOU => businessAppADOU.AuthoritativeSourceId)
           .OnDelete(DeleteBehavior.Restrict)
           .HasConstraintName("FK_BusinessAppADOU_AuthoritativeSource");

        builder.HasOne(businessAppADOU => businessAppADOU.BusinessAppResourceIdentity)
            .WithMany(source => source.BusinessAppActiveDirectoryOrganizationalUnits)
            .HasForeignKey(businessAppADOU => businessAppADOU.BusinessAppResourceIdentityId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_BusinessAppADOU_ResourceIdentity");

        builder.HasOne(businessAppADOU => businessAppADOU.Domain)
            .WithMany(source => source.BusinessAppActiveDirectoryOrganizationalUnits)
            .HasForeignKey(businessAppADOU => businessAppADOU.DomainId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_BusinessAppADOU_Domain");

        builder.HasOne(businessAppADOU => businessAppADOU.ParentOu)
            .WithMany(x => x.Children)
            .HasForeignKey(businessAppADOU => businessAppADOU.ParentOuId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_BusinessAppADOU_ParentOu");
    }
}
