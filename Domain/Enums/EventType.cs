namespace IAMBuddy.Domain.Enums;

public enum EventType
{
    IdentityCreated,
    IdentityUpdated,
    IdentityDeprovisioned,
    AccountProvisioned,
    AccountUpdated,
    AccountDeprovisioned,
    RoleAssigned,
    RoleUnassigned,
    AccessGranted,
    AccessRevoked,
    AccessReviewed,
    PolicyCreated,
    PolicyModified,
    PolicyDeleted,
    CredentialRotated,
    ApprovalRequested,
    ApprovalApproved,
    ApprovalRejected,
    AnomalyDetected,
    LoginAttempt,
    PermissionChange
}
