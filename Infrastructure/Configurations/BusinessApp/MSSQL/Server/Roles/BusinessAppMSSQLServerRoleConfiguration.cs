namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Server.Roles;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerRoleConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerRole>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerRole> builder)
    {
        builder.ToTable("BusinessAppMSSQLServerRoles");
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
