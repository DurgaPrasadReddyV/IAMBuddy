namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DeprovisionBusinessAppServiceActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<DeprovisionBusinessAppServiceActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<DeprovisionBusinessAppServiceActiveDirectoryAccount> builder)
    {
        builder.ToTable("DeprovisionBusinessAppServiceActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
