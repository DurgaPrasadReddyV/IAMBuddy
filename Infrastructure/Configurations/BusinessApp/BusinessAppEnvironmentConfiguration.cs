namespace IAMBuddy.Infrastructure.Configurations.BusinessApp;

using IAMBuddy.Domain.BusinessApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppEnvironmentConfiguration : IEntityTypeConfiguration<BusinessAppEnvironment>
{
    public void Configure(EntityTypeBuilder<BusinessAppEnvironment> builder)
    {
        builder.ToTable("BusinessAppEnvironments");

        builder.Property(x => x.Environment)
            .IsRequired();

        builder.Property(x => x.EnvironmentName)
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .HasMaxLength(512);

        builder.Property(x => x.Url)
            .HasMaxLength(256);

        builder.HasOne(x => x.BusinessApplication)
            .WithMany()
            .HasForeignKey(x => x.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
