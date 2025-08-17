namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;

using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerADHumanLoginRoleConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerADHumanLoginRole>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerADHumanLoginRole> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.BusinessAppMSSQLServerADHumanLogin)
               .WithMany()
               .HasForeignKey(e => e.BusinessAppMSSQLServerADHumanLoginId)
               .IsRequired(false);
        builder.HasOne(e => e.ServerRole)
               .WithMany()
               .IsRequired();
    }
}
