namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;

using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerADServiceLoginRoleConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerADServiceLoginRole>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerADServiceLoginRole> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.BusinessAppMSSQLServerADServiceLogin)
               .WithMany()
               .HasForeignKey(e => e.BusinessAppMSSQLServerADServiceLoginId)
               .IsRequired(false);
        builder.HasOne(e => e.ServerRole)
               .WithMany()
               .IsRequired();
    }
}
