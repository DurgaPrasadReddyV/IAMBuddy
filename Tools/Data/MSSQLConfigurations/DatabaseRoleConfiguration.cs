namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatabaseRoleConfiguration : IEntityTypeConfiguration<DatabaseRole>
{
    public void Configure(EntityTypeBuilder<DatabaseRole> builder)
    {
        builder.ToTable("DatabaseRoles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.Database)
            .WithMany(x => x.DatabaseRoles)
            .HasForeignKey(x => x.DatabaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.DatabaseId, x.Name })
            .IsUnique();
    }
}
