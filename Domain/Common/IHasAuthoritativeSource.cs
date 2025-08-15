namespace IAMBuddy.Domain.Common;
public interface IHasAuthoritativeSource
{
    public int AuthoritativeSourceId { get; set; }
    public AuthoritativeSource AuthoritativeSource { get; set; }
}
