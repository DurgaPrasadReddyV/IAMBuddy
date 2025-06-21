
var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL Server
var postgresServer = builder.AddPostgres("postgresql")
    .WithPgAdmin();

//Add Temporal Server
var temporalServer = await builder.AddTemporalServerContainer("temporal-server");
temporalServer.WaitFor(postgresServer).WithReference(postgresServer);

//Add Temporal Worker
var temporalWorker = builder.AddProject<Projects.IAMBuddy_TemporalWorker>("temporal-worker");
temporalWorker.WaitFor(temporalServer).WithReference(temporalServer);

// Add Request Intake Service
var requestIntakeService = builder.AddProject<Projects.IAMBuddy_RequestIntakeService>("request-intake-service");
requestIntakeService.WaitFor(postgresServer).WithReference(postgresServer);
requestIntakeService.WaitFor(temporalWorker).WithReference(temporalWorker);

// Add Approval Service
var approvalService = builder.AddProject<Projects.IAMBuddy_ApprovalService>("approval-service");
approvalService.WaitFor(postgresServer).WithReference(postgresServer);
approvalService.WaitFor(temporalWorker).WithReference(temporalWorker);

// Add Provisioning Service
var provisioningService = builder.AddProject<Projects.IAMBuddy_ProvisioningService>("provisioning-service");
provisioningService.WaitFor(postgresServer).WithReference(postgresServer);
provisioningService.WaitFor(temporalWorker).WithReference(temporalWorker);

// Add Redis
var redis = builder.AddRedis("redis");

// Add Notification Service
var notificationService = builder.AddProject<Projects.IAMBuddy_NotificationService>("notification-service");
notificationService.WaitFor(redis).WithReference(redis);

builder.Build().Run();
