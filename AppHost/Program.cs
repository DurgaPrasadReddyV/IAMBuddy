using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    //.WithDataVolume(isReadOnly: false)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var toolsDb = postgres.AddDatabase("ToolsDb");

var databaseGenerator = builder.AddProject<Projects.DatabaseGenerator>("database-generator")
    .WaitFor(toolsDb)
    .WithReference(toolsDb); // reference to the postgres database

// Add other services with PostgreSQL connection
builder.AddProject<Projects.Tools>("tools")
    .WaitForCompletion(databaseGenerator)
    .WithReference(toolsDb);

builder.AddProject<Projects.Agents>("agents");

builder.Build().Run();
