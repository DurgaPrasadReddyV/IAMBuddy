namespace IAMBuddy.Tools.Data.Configurations;

using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ServerLogin Configuration
public class ServerLoginConfiguration : IEntityTypeConfiguration<ServerLogin>
{
    public void Configure(EntityTypeBuilder<ServerLogin> builder)
    {
        builder.HasKey(sl => sl.Id);
        builder.Property(sl => sl.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(sl => new { sl.ServerId, sl.Name }).IsUnique(); // Login name unique per server
        builder.Property(sl => sl.DefaultDatabase).HasMaxLength(255);
        builder.Property(sl => sl.DefaultLanguage).HasMaxLength(50);
        builder.Property(sl => sl.Description).HasMaxLength(500);

        builder.HasOne(sl => sl.Server)
               .WithMany(ss => ss.ServerLogins)
               .HasForeignKey(sl => sl.ServerId)
               .OnDelete(DeleteBehavior.Restrict); // Don't delete server if logins exist

        builder.HasOne(sl => sl.ActiveDirectoryAccount)
               .WithMany(ada => ada.ServerLogins)
               .HasForeignKey(sl => sl.ActiveDirectoryAccountId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(sl => sl.ActiveDirectoryGroup)
               .WithMany(adg => adg.ServerLogins)
               .HasForeignKey(sl => sl.ActiveDirectoryGroupId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(sl => sl.NonHumanIdentity)
               .WithMany(nhi => nhi.ServerLogins)
               .HasForeignKey(sl => sl.NonHumanIdentityId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(sl => sl.DatabaseUsers)
               .WithOne(du => du.ServerLogin)
               .HasForeignKey(du => du.ServerLoginId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(sl => sl.ServerLoginRoles)
               .WithOne(slr => slr.ServerLogin)
               .HasForeignKey(slr => slr.ServerLoginId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
