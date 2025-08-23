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

        builder.HasOne(businessAppADServiceAcc => businessAppADServiceAcc.AuthoritativeSource)
            .WithMany(source => source.BusinessAppServiceActiveDirectoryAccounts)
            .HasForeignKey(businessAppADServiceAcc => businessAppADServiceAcc.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppADServiceAcc => businessAppADServiceAcc.BusinessAppResourceIdentity)
            .WithMany(source => source.BusinessAppServiceActiveDirectoryAccounts)
            .HasForeignKey(businessAppADServiceAcc => businessAppADServiceAcc.BusinessAppResourceIdentityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
