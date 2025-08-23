
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    //.WithDataVolume(isReadOnly: false)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var iamBuddyDb = postgres.AddDatabase("IAMBuddyDb");

var databaseGenerator = builder.AddProject<Projects.DatabaseGenerator>("database-generator")
    .WaitFor(iamBuddyDb)
    .WithReference(iamBuddyDb); // reference to the postgres database

// Add other services with PostgreSQL connection
builder.AddProject<Projects.Tools>("tools")
    .WaitForCompletion(databaseGenerator)
    .WithReference(iamBuddyDb);

builder.AddProject<Projects.Agents>("agents");

builder.Build().Run();
