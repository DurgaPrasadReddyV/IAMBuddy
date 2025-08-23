namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Database.Users;
using IAMBuddy.Domain.BusinessApp.MSSQL.Database.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLDatabaseADGroupUserConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLDatabaseADGroupUser>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLDatabaseADGroupUser> builder)
    {
        builder.ToTable("BusinessAppMSSQLDatabaseADGroupUsers");
        builder.HasOne(x => x.Database)
            .WithMany()
            .HasForeignKey("DatabaseId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
