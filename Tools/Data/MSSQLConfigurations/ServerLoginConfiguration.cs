namespace IAMBuddy.Tools.Data.MSSQLConfigurations;
using IAMBuddy.Tools.Data.MSSQLModels;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ServerLoginConfiguration : IEntityTypeConfiguration<ServerLogin>
{
    public void Configure(EntityTypeBuilder<ServerLogin> builder)
    {
        builder.ToTable("ServerLogins");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.AuthenticationType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.DefaultDatabase)
            .HasMaxLength(128);

        builder.Property(x => x.DefaultLanguage)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.Server)
            .WithMany(x => x.ServerLogins)
            .HasForeignKey(x => x.ServerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ServerId, x.Name })
            .IsUnique();
    }
}
