namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppServiceActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<BusinessAppServiceActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<BusinessAppServiceActiveDirectoryAccount> builder)
    {
        builder.ToTable("BusinessAppServiceActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
