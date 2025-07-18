namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SqlServerInstanceConfiguration : IEntityTypeConfiguration<SqlServerInstance>
{
    public void Configure(EntityTypeBuilder<SqlServerInstance> builder)
    {
        builder.ToTable("SqlServerInstances");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Port)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ServiceAccount)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Collation)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.Server)
            .WithMany(x => x.Instances)
            .HasForeignKey(x => x.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Listener)
            .WithMany(x => x.Instances)
            .HasForeignKey(x => x.ListenerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.ServerId, x.Name })
            .IsUnique();
    }
}
