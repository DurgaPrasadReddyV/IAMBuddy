namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Server;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServer>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServer> builder)
    {
        builder.ToTable("BusinessAppMSSQLServers");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.HostName).HasMaxLength(256);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
