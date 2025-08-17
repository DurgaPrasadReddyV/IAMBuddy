namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;
using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppServiceActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<BusinessAppServiceActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<BusinessAppServiceActiveDirectoryAccount> builder)
    {
        builder.ToTable("BusinessAppServiceActiveDirectoryAccounts");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.SamAccountName).IsRequired().HasMaxLength(64);
        builder.Property(x => x.UserPrincipalName).HasMaxLength(128);
        builder.Property(x => x.DistinguishedName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Sid).IsRequired().HasMaxLength(128);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
