namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatabaseUserConfiguration : IEntityTypeConfiguration<DatabaseUser>
{
    public void Configure(EntityTypeBuilder<DatabaseUser> builder)
    {
        builder.ToTable("DatabaseUsers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.DefaultSchema)
            .HasMaxLength(128);

        builder.Property(x => x.UserType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.Database)
            .WithMany(x => x.DatabaseUsers)
            .HasForeignKey(x => x.DatabaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ServerLogin)
            .WithMany(x => x.DatabaseUsers)
            .HasForeignKey(x => x.ServerLoginId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => new { x.DatabaseId, x.Name })
            .IsUnique();
    }
}
