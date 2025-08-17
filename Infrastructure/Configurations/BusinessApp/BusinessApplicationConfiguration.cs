namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessApplicationConfiguration : IEntityTypeConfiguration<BusinessApplication>
{
    public void Configure(EntityTypeBuilder<BusinessApplication> builder)
    {
        builder.ToTable("BusinessApplications");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .HasMaxLength(512);

        builder.Property(x => x.BusinessPurpose)
            .HasMaxLength(256);

        builder.Property(x => x.VendorName)
            .HasMaxLength(128);

        builder.Property(x => x.Version)
            .HasMaxLength(64);

        builder.Property(x => x.ComplianceRequirements)
            .HasMaxLength(256);

        builder.Property(x => x.DataClassification)
            .HasMaxLength(64);

        builder.Property(x => x.SourceCodeRepository)
            .HasMaxLength(256);

        builder.Property(x => x.DocumentationLink)
            .HasMaxLength(256);

        builder.HasOne(x => x.AuthoritativeSource)
            .WithMany()
            .HasForeignKey(x => x.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PrimaryOwner)
            .WithMany()
            .HasForeignKey(x => x.PrimaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SecondaryOwner)
            .WithMany()
            .HasForeignKey(x => x.SecondaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TechnicalContact)
            .WithMany()
            .HasForeignKey(x => x.TechnicalContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BusinessContact)
            .WithMany()
            .HasForeignKey(x => x.BusinessContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
