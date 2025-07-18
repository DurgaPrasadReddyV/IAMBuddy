namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SqlServerConfiguration : IEntityTypeConfiguration<SqlServer>
{
    public void Configure(EntityTypeBuilder<SqlServer> builder)
    {
        builder.ToTable("SqlServers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.HostName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.IPAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Edition)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ServicePack)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
