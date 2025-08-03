namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;


public class BusinessAppHumanActiveDirectoryGroupMembership : BusinessAppUserOwnedResource
{
    public DateTime? MemberSince { get; set; }
    public string? AddedBy { get; set; }
    public int GroupId { get; set; }
    public virtual BusinessAppActiveDirectoryGroup Group { get; set; } = null!;
    public int? BusinessAppHumanActiveDirectoryAccountId { get; set; }
    public virtual BusinessAppHumanActiveDirectoryAccount? BusinessAppHumanActiveDirectoryAccount { get; set; }
}
