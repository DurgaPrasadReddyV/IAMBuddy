namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;

using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Logins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerADHumanLoginConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerADHumanLogin>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerADHumanLogin> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Server)
               .WithMany()
               .IsRequired();
    }
}
