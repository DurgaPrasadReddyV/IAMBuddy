# SQL Server Management Service Architecture

## Overview

The SQL Server Management Service is a microservice within the IAMBuddy ecosystem designed to provide comprehensive SQL Server identity and access management capabilities. This document outlines the architectural decisions, design patterns, and technical implementation details.

## Table of Contents

- [System Architecture](#system-architecture)
- [Design Principles](#design-principles)
- [Layer Architecture](#layer-architecture)
- [Data Architecture](#data-architecture)
- [Integration Architecture](#integration-architecture)
- [Security Architecture](#security-architecture)
- [Scalability Considerations](#scalability-considerations)
- [Technology Stack](#technology-stack)
- [Design Patterns](#design-patterns)
- [Future Enhancements](#future-enhancements)

## System Architecture

### High-Level Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   IAMBuddy      │    │   Temporal      │    │   SQL Server    │
│   AppHost       │────│   Workflow      │    │   Instances     │
│   (Orchestrator)│    │   Engine        │    │   (Target)      │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                        │                        │
         │                        │                        │
         ▼                        ▼                        ▼
┌─────────────────────────────────────────────────────────────────┐
│              SQL Server Management Service                      │
├─────────────────────────────────────────────────────────────────┤
│  Controllers │  Services  │  Repositories │  Models  │  DTOs   │
├─────────────────────────────────────────────────────────────────┤
│              Entity Framework Core (ORM)                       │
├─────────────────────────────────────────────────────────────────┤
│              SQL Server Management Database                     │
└─────────────────────────────────────────────────────────────────┘
```

### Service Boundaries

The service manages four primary entities:
1. **SQL Server Logins** - Server-level authentication principals
2. **Server Roles** - Server-level authorization groups
3. **Database Users** - Database-level authentication principals
4. **Database Roles** - Database-level authorization groups

### Integration Points

- **Temporal Workflow Engine**: Orchestrates complex multi-step operations
- **IAMBuddy AppHost**: Service discovery and configuration management
- **SQL Server Instances**: Target systems for identity management operations
- **Management Database**: Metadata and audit trail storage

## Design Principles

### 1. Single Responsibility Principle
Each service class has a single, well-defined responsibility:
- `SqlLoginService`: Manages SQL Server logins
- `DatabaseUserService`: Manages database users
- `ServerRoleService`: Manages server roles
- `DatabaseRoleService`: Manages database roles

### 2. Separation of Concerns
Clear separation between:
- **Controllers**: HTTP request/response handling
- **Services**: Business logic implementation
- **Repositories**: Data access abstraction
- **Models**: Domain entities
- **DTOs**: Data transfer objects

### 3. Dependency Inversion
All dependencies are injected via interfaces, enabling:
- Easy unit testing
- Loose coupling
- Flexible implementation swapping

### 4. Fail-Safe Operations
- Comprehensive validation before operations
- Rollback capabilities for failed operations
- Detailed error reporting and logging

### 5. Idempotency
All operations are designed to be idempotent where possible:
- Creating existing objects returns conflict status
- Deleting non-existent objects handles gracefully
- Updates are atomic and consistent

## Layer Architecture

### Presentation Layer (Controllers)

```csharp
[ApiController]
[Route("api/[controller]")]
public class SqlLoginController : ControllerBase
{
    // HTTP request handling
    // Input validation
    // Response formatting
    // Error handling
}
```

**Responsibilities:**
- HTTP request/response handling
- Input validation and model binding
- Authentication and authorization
- Exception handling and logging
- Response formatting and status codes

### Business Logic Layer (Services)

```csharp
public interface ISqlLoginService
{
    Task<IEnumerable<SqlServerLogin>> GetLoginsAsync(string serverInstance);
    Task<SqlServerLogin> CreateLoginAsync(string loginName, string password, string serverInstance, string? defaultDatabase = null);
    // Additional business operations
}
```

**Responsibilities:**
- Business rule enforcement
- Transaction management
- Data validation
- External system integration
- Workflow orchestration

### Data Access Layer (Repositories)

```csharp
public interface ISqlLoginRepository
{
    Task<IEnumerable<SqlServerLogin>> GetAllAsync(string serverInstance);
    Task<SqlServerLogin?> GetByNameAsync(string loginName, string serverInstance);
    Task<SqlServerLogin> AddAsync(SqlServerLogin login);
    // CRUD operations
}
```

**Responsibilities:**
- Data persistence operations
- Query optimization
- Connection management
- Transaction coordination

### Domain Layer (Models)

```csharp
public class SqlServerLogin
{
    public int Id { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string LoginType { get; set; } = string.Empty;
    // Domain properties and methods
}
```

**Responsibilities:**
- Domain entity representation
- Business invariants
- Domain-specific behavior
- Relationship modeling

## Data Architecture

### Entity Relationship Diagram

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  SqlServerLogin │    │ SqlServerUser   │    │  SqlServerRole  │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ Id (PK)         │    │ Id (PK)         │    │ Id (PK)         │
│ LoginName       │◄──┐│ UserName        │    │ RoleName        │
│ LoginType       │   ││ DatabaseName    │    │ RoleType        │
│ ServerInstance  │   ││ ServerInstance  │    │ DatabaseName    │
│ Sid             │   ││ LoginId (FK)    │    │ ServerInstance  │
│ IsEnabled       │   │└─────────────────┘    │ IsBuiltIn       │
│ CreatedDate     │   │                       │ CreatedDate     │
└─────────────────┘   │ ┌─────────────────┐   └─────────────────┘
                      └─┤SqlServerRoleAssn│
                        ├─────────────────┤
                        │ Id (PK)         │
                        │ PrincipalId     │
                        │ RoleId          │
                        │ AssignmentType  │
                        └─────────────────┘
```

### Database Design Decisions

#### 1. Multi-Instance Support
- `ServerInstance` field in all entities
- Composite unique constraints including server instance
- Enables management of multiple SQL Server instances

#### 2. Audit Trail
- `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy` in all entities
- Comprehensive change tracking
- Supports compliance and debugging

#### 3. Flexible Role Assignments
- `SqlServerRoleAssignment` handles both server and database role assignments
- `AssignmentType` discriminates between different assignment types
- Supports complex hierarchical role structures

#### 4. Security Identifiers (SIDs)
- Store SQL Server SIDs for precise identification
- Handles scenarios where names might be reused
- Ensures referential integrity across operations

### Data Access Patterns

#### Repository Pattern
```csharp
public class SqlLoginRepository : ISqlLoginRepository
{
    private readonly SqlServerManagementDbContext _context;
    
    public async Task<IEnumerable<SqlServerLogin>> GetAllAsync(string serverInstance)
    {
        return await _context.SqlServerLogins
            .Where(l => l.ServerInstance == serverInstance)
            .OrderBy(l => l.LoginName)
            .ToListAsync();
    }
}
```

#### Unit of Work Pattern
```csharp
public class SqlLoginService : ISqlLoginService
{
    public async Task<SqlServerLogin> CreateLoginAsync(string loginName, string password, string serverInstance, string? defaultDatabase = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Business logic
            var result = await _repository.AddAsync(login);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

## Integration Architecture

### Temporal Workflow Integration

```csharp
public class SqlServerProvisioningWorkflow
{
    [WorkflowRun]
    public async Task<WorkflowResult> ExecuteAsync(ProvisioningRequest request)
    {
        // Orchestrate multi-step provisioning
        var login = await Workflow.ExecuteActivityAsync(
            (SqlServerActivities act) => act.CreateLoginAsync(request.LoginDetails));
            
        var user = await Workflow.ExecuteActivityAsync(
            (SqlServerActivities act) => act.CreateUserAsync(request.UserDetails));
            
        await Workflow.ExecuteActivityAsync(
            (SqlServerActivities act) => act.AssignRolesAsync(request.RoleAssignments));
            
        return new WorkflowResult { Success = true };
    }
}
```

### Service-to-Service Communication

#### Direct Database Operations
- Services connect directly to target SQL Server instances
- Uses `SqlServerConnectionService` for connection management
- Implements connection pooling and retry policies

#### Metadata Operations
- Uses Entity Framework for local database operations
- Maintains audit trail and operation history
- Supports query optimization and caching

### External System Integration

#### SQL Server Connection Management
```csharp
public class SqlServerConnectionService : ISqlServerConnectionService
{
    public async Task<IDbConnection> GetConnectionAsync(string serverInstance, string? database = null)
    {
        var connectionString = BuildConnectionString(serverInstance, database);
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
```

#### Error Handling and Retry Logic
```csharp
public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch (SqlException ex) when (IsTransientError(ex) && i < maxRetries - 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
        }
    }
    throw new InvalidOperationException("Max retries exceeded");
}
```

## Security Architecture

### Authentication and Authorization

#### Service Authentication
- Service-to-service authentication via JWT tokens
- Temporal client authentication with TLS certificates
- SQL Server authentication using service accounts

#### API Security
```csharp
[Authorize(Roles = "SqlServerAdmin")]
[HttpPost]
public async Task<ActionResult<SqlLoginDto>> CreateLogin([FromBody] CreateSqlLoginRequest request)
{
    // Implementation
}
```

### Data Protection

#### Connection String Security
- Encrypted configuration storage
- Environment-specific secrets management
- Principle of least privilege for service accounts

#### Input Validation
```csharp
public class CreateSqlLoginRequest
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9_\-\\]+$")]
    public string LoginName { get; set; } = string.Empty;
    
    [StringLength(128)]
    public string? Password { get; set; }
}
```

#### SQL Injection Prevention
- Parameterized queries for all database operations
- Input sanitization and validation
- Use of stored procedures where appropriate

### Audit and Compliance

#### Comprehensive Logging
```csharp
public async Task<SqlServerLogin> CreateLoginAsync(string loginName, string password, string serverInstance, string? defaultDatabase = null)
{
    _logger.LogInformation("Creating login {LoginName} on server {ServerInstance}", 
        loginName, serverInstance);
    
    try
    {
        var result = await PerformCreateLogin(loginName, password, serverInstance, defaultDatabase);
        
        _logger.LogInformation("Successfully created login {LoginName} on server {ServerInstance}", 
            loginName, serverInstance);
        
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create login {LoginName} on server {ServerInstance}", 
            loginName, serverInstance);
        throw;
    }
}
```

#### Change Tracking
- All operations tracked with timestamps and user context
- Immutable audit logs
- Compliance reporting capabilities

## Scalability Considerations

### Horizontal Scaling

#### Stateless Design
- No session state stored in service instances
- All state persisted in databases
- Enables multiple service instances

#### Load Balancing
- HTTP load balancing for API requests
- Database connection pooling
- Temporal worker scaling

### Performance Optimization

#### Database Optimization
```sql
-- Efficient indexing strategy
CREATE NONCLUSTERED INDEX IX_SqlServerLogin_ServerInstance_LoginName 
ON SqlServerLogins (ServerInstance, LoginName);

CREATE NONCLUSTERED INDEX IX_SqlServerUser_DatabaseName_ServerInstance 
ON SqlServerUsers (DatabaseName, ServerInstance);
```

#### Caching Strategy
```csharp
public class CachedSqlLoginService : ISqlLoginService
{
    private readonly ISqlLoginService _inner;
    private readonly IMemoryCache _cache;
    
    public async Task<SqlServerLogin?> GetLoginAsync(string loginName, string serverInstance)
    {
        var cacheKey = $"login:{serverInstance}:{loginName}";
        if (_cache.TryGetValue(cacheKey, out SqlServerLogin? cached))
        {
            return cached;
        }
        
        var result = await _inner.GetLoginAsync(loginName, serverInstance);
        if (result != null)
        {
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        }
        
        return result;
    }
}
```

#### Async/Await Pattern
- All I/O operations are asynchronous
- Non-blocking operation execution
- Improved throughput and responsiveness

### Resource Management

#### Connection Pooling
```csharp
services.AddDbContext<SqlServerManagementDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(300);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
}, ServiceLifetime.Scoped);
```

#### Memory Management
- Proper disposal of database connections
- Limited cache sizes with LRU eviction
- Garbage collection optimization

## Technology Stack

### Core Technologies
- **.NET 9.0**: Latest .NET version with performance improvements
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 9.0**: ORM for database operations
- **SQL Server**: Primary database storage

### Integration Technologies
- **Temporal.io**: Workflow orchestration engine
- **Dapper**: Lightweight ORM for complex queries
- **OpenTelemetry**: Observability and tracing
- **Serilog/Microsoft.Extensions.Logging**: Structured logging

### Development Tools
- **Visual Studio 2022**: Primary IDE
- **Docker**: Containerization
- **Azure DevOps/GitHub Actions**: CI/CD pipelines
- **SonarQube**: Code quality analysis

## Design Patterns

### 1. Repository Pattern
Abstracts data access logic and provides a uniform interface for data operations.

### 2. Service Layer Pattern
Encapsulates business logic and provides a clear API for business operations.

### 3. DTO Pattern
Separates internal domain models from external API contracts.

### 4. Factory Pattern
Creates database connections and service instances based on configuration.

### 5. Strategy Pattern
Handles different SQL Server authentication methods and connection types.

### 6. Observer Pattern
Implements event-driven notifications for operation completion.

### 7. Command Pattern
Encapsulates operations as objects for queuing, logging, and undo operations.

## Future Enhancements

### Planned Features

#### 1. Advanced Security Features
- Multi-factor authentication support
- Advanced password policies
- Privilege escalation detection

#### 2. High Availability
- Active-passive failover
- Database replication support
- Circuit breaker patterns

#### 3. Enhanced Monitoring
- Real-time dashboards
- Anomaly detection
- Performance analytics

#### 4. Integration Enhancements
- REST API versioning
- GraphQL support
- Event streaming with Apache Kafka

#### 5. Advanced Workflow Support
- Complex approval processes
- Conditional provisioning
- Automated deprovisioning

### Technical Debt Items

1. **Performance**: Implement more sophisticated caching strategies
2. **Security**: Add comprehensive authorization framework
3. **Testing**: Increase test coverage to >90%
4. **Documentation**: Add architectural decision records (ADRs)
5. **Monitoring**: Implement custom metrics and alerting

## Conclusion

The SQL Server Management Service is designed as a robust, scalable, and secure microservice that effectively manages SQL Server identities and access controls. The architecture supports current requirements while providing flexibility for future enhancements and integration scenarios.

The service follows established architectural patterns and best practices, ensuring maintainability, testability, and reliability. The modular design enables independent development and deployment while maintaining strong integration with the broader IAMBuddy ecosystem.