namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;

using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Logins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerSQLAccountLoginConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerSQLAccountLogin>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerSQLAccountLogin> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Server)
               .WithMany()
               .IsRequired();
    }
}
