using Aspire.Hosting;
using InfinityFlow.Aspire.Temporal;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server for database services
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

// Add databases
var requestIntakeDb = sqlServer.AddDatabase("iambuddy-requestintake");
var sqlServerManagementDb = sqlServer.AddDatabase("iambuddy-sqlservermanagement");

var temporal = await builder.AddTemporalServerContainer("temporal", x => x
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2")
    .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true)
);

var temporalworker = builder.AddProject<Projects.IAMBuddy_TemporalWorker>("iambuddy-temporalworker")
    .WithReference(temporal);

builder.AddProject<Projects.IAMBuddy_RequestIntakeService>("iambuddy-requestintakeservice")
    .WithReference(temporal)
    .WithReference(requestIntakeDb)
    .WaitFor(temporalworker);

builder.AddProject<Projects.IAMBuddy_ApprovalService>("iambuddy-approvalservice")
    .WithReference(temporal).WaitFor(temporalworker);

builder.AddProject<Projects.IAMBuddy_ProvisioningService>("iambuddy-provisioningservice")
    .WithReference(temporal).WaitFor(temporalworker);

builder.AddProject<Projects.IAMBuddy_NotificationService>("iambuddy-notificationservice");

builder.AddProject<Projects.IAMBuddy_SqlServerManagementService>("iambuddy-sqlservermanagementservice")
    .WithReference(sqlServerManagementDb)
    .WithReference(temporal);

builder.Build().Run();
