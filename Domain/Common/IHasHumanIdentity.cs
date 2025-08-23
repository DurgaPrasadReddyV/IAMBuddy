namespace IAMBuddy.Domain.Common;
public interface IHasHumanIdentity
{
    public int HumanIdentityId { get; set; }
    public HumanIdentity HumanIdentity { get; set; }
}
