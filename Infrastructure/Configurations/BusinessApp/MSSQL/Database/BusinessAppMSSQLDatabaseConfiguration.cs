namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Database;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLDatabaseConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLDatabase>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLDatabase> builder)
    {
        builder.ToTable("BusinessAppMSSQLDatabases");
        builder.HasOne(x => x.Instance)
            .WithMany()
            .HasForeignKey("InstanceId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
