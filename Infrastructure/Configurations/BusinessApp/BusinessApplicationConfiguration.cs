namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessApplicationConfiguration : IEntityTypeConfiguration<BusinessApplication>
{
    public void Configure(EntityTypeBuilder<BusinessApplication> builder)
    {
        builder.ToTable("BusinessApplications");

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessApp => businessApp.BusinessContact)
            .WithOne()
            .HasForeignKey<BusinessApplication>(businessApp => businessApp.BusinessContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.TechnicalContact)
            .WithOne()
            .HasForeignKey<BusinessApplication>(businessApp => businessApp.TechnicalContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.PrimaryOwner)
            .WithOne()
            .HasForeignKey<BusinessApplication>(businessApp => businessApp.PrimaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.SecondaryOwner)
            .WithOne()
            .HasForeignKey<BusinessApplication>(businessApp => businessApp.SecondaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessApp => businessApp.AuthoritativeSource)
            .WithMany(source => source.BusinessApplications)
            .HasForeignKey(businessApp => businessApp.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
