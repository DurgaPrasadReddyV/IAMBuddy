namespace IAMBuddy.Domain.BusinessApp.ActiveDirectory;

using IAMBuddy.Domain.Common;

public class BusinessAppHumanActiveDirectoryGroupMembership : HumanIdentityOwnedResource
{
    public DateTime? MemberSince { get; set; }
    public string? AddedBy { get; set; }
    public int GroupId { get; set; }
    public virtual BusinessAppActiveDirectoryGroup Group { get; set; } = null!;
    public int? BusinessAppHumanActiveDirectoryAccountId { get; set; }
    public virtual BusinessAppHumanActiveDirectoryAccount? BusinessAppHumanActiveDirectoryAccount { get; set; }
}
