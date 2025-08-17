namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProvisionBusinessAppServiceActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<ProvisionBusinessAppServiceActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<ProvisionBusinessAppServiceActiveDirectoryAccount> builder)
    {
        builder.ToTable("ProvisionBusinessAppServiceActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
