namespace IAMBuddy.Domain.BusinessApp;

using System;
using System.ComponentModel.DataAnnotations;
using IAMBuddy.Domain.Common;

public class BusinessAppEnvironment : IAuditableEntity, IHasBusinessApplication
{
    public EBusinessAppEnvironment Environment { get; set; }
    public string? EnvironmentName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? Url { get; set; }

    // IHasBusinessApplication
    public int BusinessApplicationId { get; set; }
    public virtual BusinessApplication BusinessApplication { get; set; } = null!;

    // IAuditableEntity
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    [Timestamp] public byte[]? RowVersion { get; set; }


    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public enum EBusinessAppEnvironment
    {
        Production = 1,
        UAT = 2,
        Test = 3,
        Development = 4,
        Integration = 5,
        Staging = 6
    }
}
