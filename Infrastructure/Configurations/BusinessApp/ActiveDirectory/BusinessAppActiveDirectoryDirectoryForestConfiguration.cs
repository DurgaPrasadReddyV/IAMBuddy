namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryDirectoryForestConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryDirectoryForest>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryDirectoryForest> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryDirectoryForests");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.RootDomainName).HasMaxLength(256);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
