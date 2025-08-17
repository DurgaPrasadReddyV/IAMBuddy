namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;

using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Logins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerADServiceLoginConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerADServiceLogin>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerADServiceLogin> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Server)
               .WithMany()
               .IsRequired();
    }
}
