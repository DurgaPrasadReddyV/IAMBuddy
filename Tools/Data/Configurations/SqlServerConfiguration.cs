namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// SqlServer Configuration
public class SqlServerConfiguration : IEntityTypeConfiguration<SqlServer>
{
    public void Configure(EntityTypeBuilder<SqlServer> builder)
    {
        builder.HasKey(ss => ss.Id);
        builder.Property(ss => ss.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(ss => ss.Name).IsUnique();
        builder.Property(ss => ss.HostName).IsRequired().HasMaxLength(255);
        builder.HasIndex(ss => ss.HostName).IsUnique();
        builder.Property(ss => ss.IPAddress).IsRequired().HasMaxLength(50);
        builder.Property(ss => ss.Version).IsRequired().HasMaxLength(50);
        builder.Property(ss => ss.Edition).IsRequired().HasMaxLength(100);
        builder.Property(ss => ss.ServicePack).HasMaxLength(50);
        builder.Property(ss => ss.Description).HasMaxLength(500);

        builder.HasMany(ss => ss.Instances)
               .WithOne(ssi => ssi.Server)
               .HasForeignKey(ssi => ssi.ServerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ss => ss.ServerLogins)
               .WithOne(sl => sl.Server)
               .HasForeignKey(sl => sl.ServerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ss => ss.ServerRoles)
               .WithOne(sr => sr.Server)
               .HasForeignKey(sr => sr.ServerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
