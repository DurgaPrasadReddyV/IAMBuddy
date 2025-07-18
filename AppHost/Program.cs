var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Tools>("tools");

builder.AddProject<Projects.Agents>("agents");

builder.Build().Run();
