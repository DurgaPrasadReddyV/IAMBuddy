namespace IAMBuddy.Infrastructure.Configurations.BusinessApp.MSSQL.Server;
using IAMBuddy.Domain.BusinessApp.MSSQL.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BusinessAppMSSQLServerInstanceConfiguration : IEntityTypeConfiguration<BusinessAppMSSQLServerInstance>
{
    public void Configure(EntityTypeBuilder<BusinessAppMSSQLServerInstance> builder)
    {
        builder.ToTable("BusinessAppMSSQLServerInstances");
        builder.HasOne(x => x.Server)
            .WithMany()
            .HasForeignKey("ServerId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Listener)
            .WithMany()
            .HasForeignKey("ListenerId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
