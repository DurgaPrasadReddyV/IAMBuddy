namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProvisionBusinessAppHumanActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<ProvisionBusinessAppHumanActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<ProvisionBusinessAppHumanActiveDirectoryAccount> builder)
    {
        builder.ToTable("ProvisionBusinessAppHumanActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
