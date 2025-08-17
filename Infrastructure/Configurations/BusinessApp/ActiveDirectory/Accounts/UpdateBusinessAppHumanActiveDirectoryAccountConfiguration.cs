namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UpdateBusinessAppHumanActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<UpdateBusinessAppHumanActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<UpdateBusinessAppHumanActiveDirectoryAccount> builder)
    {
        builder.ToTable("UpdateBusinessAppHumanActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
