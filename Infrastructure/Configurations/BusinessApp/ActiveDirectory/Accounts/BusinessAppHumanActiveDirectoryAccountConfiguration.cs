namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppHumanActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<BusinessAppHumanActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<BusinessAppHumanActiveDirectoryAccount> builder)
    {
        builder.ToTable("BusinessAppHumanActiveDirectoryAccounts");

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(businessAppADHumanAcc => businessAppADHumanAcc.AuthoritativeSource)
            .WithMany(source => source.BusinessAppHumanActiveDirectoryAccounts)
            .HasForeignKey(businessAppADHumanAcc => businessAppADHumanAcc.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(businessAppADHumanAcc => businessAppADHumanAcc.BusinessAppUser)
            .WithMany(source => source.BusinessAppHumanActiveDirectoryAccounts)
            .HasForeignKey(businessAppADHumanAcc => new { businessAppADHumanAcc.BusinessApplicationId, businessAppADHumanAcc.HumanIdentityId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
