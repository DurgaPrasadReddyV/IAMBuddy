namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.ActiveDirectory.Accounts;

using IAMBuddy.Domain.BusinessApp.ActiveDirectory.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppHumanActiveDirectoryAccountConfiguration : IEntityTypeConfiguration<BusinessAppHumanActiveDirectoryAccount>
{
    public void Configure(EntityTypeBuilder<BusinessAppHumanActiveDirectoryAccount> builder)
    {
        builder.ToTable("BusinessAppHumanActiveDirectoryAccounts");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.SamAccountName).IsRequired().HasMaxLength(64);
        builder.Property(x => x.UserPrincipalName).HasMaxLength(128);
        builder.Property(x => x.DistinguishedName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Sid).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Email).HasMaxLength(128);
        builder.Property(x => x.DisplayName).HasMaxLength(128);
        builder.Property(x => x.GivenName).HasMaxLength(64);
        builder.Property(x => x.Surname).HasMaxLength(64);
        builder.Property(x => x.ManagerSid).HasMaxLength(128);
        builder.Property(x => x.EmployeeId).HasMaxLength(32);
        builder.Property(x => x.Department).HasMaxLength(64);
        builder.Property(x => x.Title).HasMaxLength(64);

        builder.HasOne(x => x.AuthoritativeSource)
            .WithMany()
            .HasForeignKey(x => x.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BusinessApplication)
            .WithMany()
            .HasForeignKey(x => x.BusinessApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.BusinessAppUser)
            .WithMany()
            .HasForeignKey(x => x.BusinessAppUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Domain)
            .WithMany()
            .HasForeignKey(x => x.DomainId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.OrganizationalUnit)
            .WithMany()
            .HasForeignKey(x => x.OrganizationalUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
