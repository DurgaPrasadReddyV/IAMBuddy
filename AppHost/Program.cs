
using NorthernNerds.Aspire.Hosting.Neo4j;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    //.WithDataVolume(isReadOnly: false)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

var iamBuddyDb = postgres.AddDatabase("IAMBuddyDb");

var neo4jPass = builder.AddParameter("neo4j-pass", secret: true);
var neo4jUser = builder.AddParameter("neo4j-user", secret: true);
var qdrantApiKey = builder.AddParameter("qdrant-api-key", secret: true);

var neo4jDb = builder.AddNeo4j("graph-db", neo4jUser, neo4jPass);
var qdrant = builder.AddQdrant("qdrant", qdrantApiKey)
    .WithLifetime(ContainerLifetime.Persistent);

var databaseGenerator = builder.AddProject<Projects.DatabaseGenerator>("database-generator")
    .WaitFor(iamBuddyDb)
    .WithReference(iamBuddyDb); // reference to the postgres database

#pragma warning disable ASPIREHOSTINGPYTHON001
var cognee = builder.AddPythonApp("cognee", "../Cognee", "Cognee.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints()
       .WithOtlpExporter();
#pragma warning restore ASPIREHOSTINGPYTHON001

// Add other services with PostgreSQL connection
builder.AddProject<Projects.Tools>("tools")
    .WaitForCompletion(databaseGenerator)
    .WithReference(iamBuddyDb);

builder.AddProject<Projects.Agents>("agents");

builder.Build().Run();
