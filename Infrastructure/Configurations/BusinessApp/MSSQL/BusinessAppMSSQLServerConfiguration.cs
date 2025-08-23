namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL;

using IAMBuddy.Domain.BusinessApp.MSSQL;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerListenerConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerListener>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerListener> builder)
    {
        builder.ToTable("BusinessAppMSSQLServerListeners");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
