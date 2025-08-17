namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppUserConfiguration : IEntityTypeConfiguration<BusinessAppUser>
{
    public void Configure(EntityTypeBuilder<BusinessAppUser> builder)
    {
        builder.ToTable("BusinessAppUsers");

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.StartDate);
        builder.Property(x => x.EndDate);

        builder.HasOne(x => x.BusinessApplication)
            .WithMany()
            .HasForeignKey(x => x.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HumanIdentity)
            .WithMany()
            .HasForeignKey(x => x.HumanIdentityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
