namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryDirectoryDomainConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryDirectoryDomain>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryDirectoryDomain> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryDirectoryDomains");

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppADDomain => businessAppADDomain.AuthoritativeSource)
            .WithMany(source => source.BusinessAppActiveDirectoryDirectoryDomains)
            .HasForeignKey(businessAppADDomain => businessAppADDomain.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppADDomain => businessAppADDomain.BusinessAppResourceIdentity)
            .WithMany(source => source.BusinessAppActiveDirectoryDirectoryDomains)
            .HasForeignKey(businessAppADDomain => businessAppADDomain.BusinessAppResourceIdentityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppADDomain => businessAppADDomain.Forest)
            .WithMany(source => source.BusinessAppActiveDirectoryDirectoryDomains)
            .HasForeignKey(businessAppADDomain => businessAppADDomain.ForestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
