namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ServerLoginRoleConfiguration : IEntityTypeConfiguration<ServerLoginRole>
{
    public void Configure(EntityTypeBuilder<ServerLoginRole> builder)
    {
        builder.ToTable("ServerLoginRoles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssignedBy)
            .HasMaxLength(128);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.ServerLogin)
            .WithMany(x => x.ServerLoginRoles)
            .HasForeignKey(x => x.ServerLoginId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ServerRole)
            .WithMany(x => x.ServerLoginRoles)
            .HasForeignKey(x => x.ServerRoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ServerLoginId, x.ServerRoleId })
            .IsUnique();
    }
}
