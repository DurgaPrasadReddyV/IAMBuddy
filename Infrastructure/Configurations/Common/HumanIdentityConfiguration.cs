namespace IAMBuddy.Infrastructure.Configurations.Common;

using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class HumanIdentityConfiguration : IEntityTypeConfiguration<HumanIdentity>
{
    public void Configure(EntityTypeBuilder<HumanIdentity> builder)
    {
        builder.ToTable("HumanIdentities");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(x => x.Phone)
            .HasMaxLength(32);
        builder.Property(x => x.JobTitle)
            .HasMaxLength(64);
        builder.Property(x => x.Department)
            .HasMaxLength(64);
        builder.Property(x => x.Division)
            .HasMaxLength(64);
        builder.Property(x => x.CostCenter)
            .HasMaxLength(32);
        builder.Property(x => x.Location)
            .HasMaxLength(64);
        builder.Property(x => x.EmployeeId)
            .HasMaxLength(32);
        builder.Property(x => x.Company)
            .HasMaxLength(64);
        builder.Property(x => x.Description)
            .HasMaxLength(256);

        builder.HasOne(x => x.Manager)
            .WithMany()
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AuthoritativeSource)
            .WithMany()
            .HasForeignKey(x => x.AuthoritativeSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
