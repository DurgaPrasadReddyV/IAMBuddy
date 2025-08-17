namespace IAMBuddy.Infrastructure.Configurations.Common;

using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuthoritativeSourceConfiguration : IEntityTypeConfiguration<AuthoritativeSource>
{
    public void Configure(EntityTypeBuilder<AuthoritativeSource> builder)
    {
        builder.ToTable("AuthoritativeSources");

        builder.Property(x => x.SourceName)
            .HasMaxLength(128);

        builder.Property(x => x.SourceType)
            .HasMaxLength(64);

        builder.Property(x => x.Description)
            .HasMaxLength(512);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
