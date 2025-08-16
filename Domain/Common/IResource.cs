namespace IAMBuddy.Domain.Common;

public interface IResource : IAuditableEntity, IHasAuthoritativeSource
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public EResourceType ResourceType { get; set; }

    public enum EResourceType
    {
        BusinessApplication = 1,
        // Platforms
        ActiveDirectoryInstance = 2,
        SqlServerPlatform = 3,
        SqlServerInstance = 4,
        SqlServerListener = 5,
        DatabasePlatform = 6,
        LinuxSystem = 7,
        NetworkingDevice = 8,
        AzureSubscription = 9,
        GcpProject = 10,
        AwsAccount = 11,
        OracleDatabase = 12,
        PostgreSqlDatabase = 13,
        // Accounts
        HumanActiveDirectoryAccount = 14,
        ServiceActiveDirectoryAccount = 15,
        HumanSqlServerLogin = 16,
        ServiceSqlServerLogin = 17,
        LinuxHumanAccount = 18,
        LinuxServiceAccount = 19,
        CloudHumanAccount = 20,
        CloudServiceAccount = 21,
        OracleHumanAccount = 22,
        OracleServiceAccount = 23,
        PostgreSqlHumanAccount = 24,
        PostgreSqlServiceAccount = 25,
        ActiveDirectoryGroup = 26,
        SqlServerServerRole = 27,
        SqlServerDatabaseRole = 28,
        LinuxGroup = 29,
        CloudRole = 30,
        OracleRole = 31,
        PostgreSqlRole = 32
    }
}
