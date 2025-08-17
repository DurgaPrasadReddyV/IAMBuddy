namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppResourceIdentityConfiguration : IEntityTypeConfiguration<BusinessAppResourceIdentity>
{
    public void Configure(EntityTypeBuilder<BusinessAppResourceIdentity> builder)
    {
        builder.ToTable("BusinessAppResourceIdentities");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(x => x.DisplayName)
            .HasMaxLength(128);
        builder.Property(x => x.Purpose)
            .HasMaxLength(256);
        builder.Property(x => x.TechnicalContact)
            .HasMaxLength(128);
        builder.Property(x => x.Description)
            .HasMaxLength(512);
        builder.Property(x => x.AccessFrequency)
            .HasMaxLength(64);

        builder.HasOne(x => x.BusinessApplication)
            .WithMany()
            .HasForeignKey(x => x.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BusinessAppEnvironment)
            .WithMany()
            .HasForeignKey(x => x.BusinessAppEnvironmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PrimaryOwner)
            .WithMany()
            .HasForeignKey(x => x.PrimaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SecondaryOwner)
            .WithMany()
            .HasForeignKey(x => x.SecondaryOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
