namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerADGroupLoginRoleConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerADGroupLoginRole>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerADGroupLoginRole> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.BusinessAppMSSQLServerADGroupLogin)
               .WithMany()
               .HasForeignKey(e => e.BusinessAppMSSQLServerADGroupLoginId)
               .IsRequired(false);
        builder.HasOne(e => e.ServerRole)
               .WithMany()
               .IsRequired();
    }
}
