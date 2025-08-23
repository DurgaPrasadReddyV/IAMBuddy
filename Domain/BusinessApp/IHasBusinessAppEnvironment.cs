namespace IAMBuddy.Domain.Common;
using IAMBuddy.Domain.BusinessApp;

internal interface IHasBusinessAppEnvironment
{
    public int BusinessAppEnvironmentId { get; set; }
    public BusinessAppEnvironment BusinessAppEnvironment { get; set; }
}
