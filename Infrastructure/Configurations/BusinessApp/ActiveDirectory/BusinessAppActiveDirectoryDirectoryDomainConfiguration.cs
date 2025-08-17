namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryDirectoryDomainConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryDirectoryDomain>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryDirectoryDomain> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryDirectoryDomains");

        builder.Property(x => x.DnsName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.NetBiosName).IsRequired().HasMaxLength(64);
        builder.Property(x => x.DomainSid).HasMaxLength(128);
        builder.Property(x => x.DistinguishedName).HasMaxLength(256);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);

        builder.HasOne(x => x.Forest)
            .WithMany()
            .HasForeignKey(x => x.ForestId)
            .OnDelete(DeleteBehavior.Restrict);
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

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
