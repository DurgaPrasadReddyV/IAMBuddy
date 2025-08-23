namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UpdateBusinessAppServiceActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<UpdateBusinessAppServiceActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<UpdateBusinessAppServiceActiveDirectoryAccount> builder)
    {
        builder.ToTable("UpdateBusinessAppServiceActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
