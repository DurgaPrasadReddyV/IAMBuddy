namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Database.Roles;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLDatabaseRoleConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLDatabaseRole>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLDatabaseRole> builder)
    {
        builder.ToTable("BusinessAppMSSQLDatabaseRoles");
        builder.HasOne(x => x.Database)
            .WithMany()
            .HasForeignKey("DatabaseId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
