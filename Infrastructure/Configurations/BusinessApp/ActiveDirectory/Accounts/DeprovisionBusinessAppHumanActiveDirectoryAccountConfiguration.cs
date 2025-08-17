namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DeprovisionBusinessAppHumanActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<DeprovisionBusinessAppHumanActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<DeprovisionBusinessAppHumanActiveDirectoryAccount> builder)
    {
        builder.ToTable("DeprovisionBusinessAppHumanActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
