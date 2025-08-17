namespace IAMBuddy.Infrastructure.Configurations.Common;

using IAMBuddy.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApprovalRequestConfiguration : IEntityTypeConfiguration<ApprovalRequest>
{
    public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
    {
        builder.ToTable("ApprovalRequests");

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(approval => approval.ApprovedBy)
            .WithMany(human => human.ApprovalRequests)
            .HasForeignKey(approval => approval.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
