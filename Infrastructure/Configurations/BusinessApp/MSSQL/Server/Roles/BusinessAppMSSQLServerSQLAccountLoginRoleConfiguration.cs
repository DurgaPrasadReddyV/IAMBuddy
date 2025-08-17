namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;

using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerSQLAccountLoginRoleConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerSQLAccountLoginRole>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerSQLAccountLoginRole> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.BusinessAppMSSQLServerSQLAccountLogin)
               .WithMany()
               .HasForeignKey(e => e.BusinessAppMSSQLServerSQLAccountLoginId)
               .IsRequired(false);
        builder.HasOne(e => e.ServerRole)
               .WithMany()
               .IsRequired();
    }
}
