using IAMBuddy.MSSQLAccountManager.Data;
using IAMBuddy.MSSQLAccountManager.Services;
using IAMBuddy.MSSQLAccountManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/mssql-account-manager-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<MSSQLAccountManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Register services
builder.Services.AddScoped<ISqlLoginService, SqlLoginService>();
builder.Services.AddScoped<IServerRoleService, ServerRoleService>();
builder.Services.AddScoped<IDatabaseUserService, DatabaseUserService>();
builder.Services.AddScoped<IDatabaseRoleService, DatabaseRoleService>();
builder.Services.AddScoped<IServerDiscoveryService, ServerDiscoveryService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
try 
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MSSQLAccountManagerContext>();
    Log.Information("Attempting to create database...");
    context.Database.EnsureCreated();
    Log.Information("Database created or already exists");
}
catch (Exception ex)
{
    Log.Warning(ex, "Failed to create database, continuing without database operations");
}

try
{
    Log.Information("Starting MSSQL Account Manager service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}