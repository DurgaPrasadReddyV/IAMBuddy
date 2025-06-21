namespace IAMBuddy.Shared.Models
{
    public enum AccountRequestStatus
    {
        Initiated,
        PendingApproval,
        Approved,
        Rejected,
        Provisioned,
        Failed,
        Abandoned
    }
}
