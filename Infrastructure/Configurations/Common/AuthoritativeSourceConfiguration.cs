namespace IAMBuddy.Infrastructure.Configurations.Common;

using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuthoritativeSourceConfiguration : IEntityTypeConfiguration<AuthoritativeSource>
{
    public void Configure(EntityTypeBuilder<AuthoritativeSource> builder)
    {
        builder.ToTable("AuthoritativeSources");

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
