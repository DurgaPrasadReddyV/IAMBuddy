namespace IAMBuddy.Infrastructure.Configurations.Common;

using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class HumanIdentityConfiguration : IEntityTypeConfiguration<HumanIdentity>
{
    public void Configure(EntityTypeBuilder<HumanIdentity> builder)
    {
        builder.ToTable("HumanIdentities", t => t.HasCheckConstraint("CK_HumanIdentities_NoSelfManager",
                "\"ManagerId\" IS NULL OR \"Id\" <> \"ManagerId\""));

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(human => human.Manager).WithMany(mgr => mgr.DirectReports)
            .HasForeignKey(human => human.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(human => human.AuthoritativeSource)
            .WithMany(source => source.HumanIdentities)
            .HasForeignKey(human => human.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
