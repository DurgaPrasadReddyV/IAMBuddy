namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppActiveDirectoryDirectoryForestConfiguration : IEntityTypeConfiguration<BusinessAppActiveDirectoryDirectoryForest>
{
    public void Configure(EntityTypeBuilder<BusinessAppActiveDirectoryDirectoryForest> builder)
    {
        builder.ToTable("BusinessAppActiveDirectoryDirectoryForests");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
