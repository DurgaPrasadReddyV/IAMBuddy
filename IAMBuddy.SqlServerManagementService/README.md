# IAMBuddy SQL Server Management Service

A comprehensive SQL Server Identity and Access Management service built with .NET 9 and Entity Framework Core. This service provides RESTful APIs for managing SQL Server logins, users, roles, and permissions across multiple SQL Server instances.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Usage Examples](#usage-examples)
- [Testing](#testing)
- [Security](#security)
- [Performance](#performance)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)

## Features

- **SQL Server Login Management**: Create, update, delete, and manage SQL Server logins
- **Database User Management**: Manage database users and their associations with logins
- **Role Management**: Handle both server-level and database-level roles
- **Permission Management**: Grant and revoke permissions at various levels
- **Multi-Instance Support**: Manage multiple SQL Server instances
- **Bulk Operations**: Perform operations on multiple objects simultaneously
- **Audit Trail**: Track all changes with created/modified metadata
- **Health Monitoring**: Built-in health checks and observability
- **Temporal Integration**: Workflow orchestration with Temporal.io

## Prerequisites

- .NET 9.0 SDK or later
- SQL Server 2019 or later (or SQL Server Express LocalDB for development)
- Visual Studio 2022 or VS Code with C# extension
- Docker (optional, for containerized deployment)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/iambuddy.git
cd iambuddy/IAMBuddy.SqlServerManagementService
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SqlServerManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Run Database Migrations

```bash
dotnet ef database update
```

### 5. Run the Service

```bash
dotnet run
```

The service will be available at `https://localhost:7020`.

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SqlServerManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true",
    "temporal": "localhost:7233"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to `Development`, `Staging`, or `Production`
- `ASPNETCORE_URLS`: Override default URLs (e.g., `https://localhost:7020`)
- `ConnectionStrings__DefaultConnection`: Override database connection string
- `ConnectionStrings__temporal`: Temporal server connection string

## API Documentation

### Base URL

```
https://localhost:7020/api
```

### Authentication

Currently, the service runs without authentication for development. In production, implement your preferred authentication mechanism (JWT, OAuth2, etc.).

### SQL Login Management

#### Get All Logins

```http
GET /api/SqlLogin/server/{serverInstance}
```

**Response:**
```json
[
  {
    "id": 1,
    "loginName": "testuser",
    "loginType": "SQL",
    "sid": "0x123456789ABCDEF",
    "isEnabled": true,
    "isLocked": false,
    "passwordExpiryDate": null,
    "lastLoginDate": "2024-01-15T10:30:00Z",
    "serverInstance": "localhost",
    "createdDate": "2024-01-01T09:00:00Z",
    "modifiedDate": "2024-01-15T10:30:00Z",
    "createdBy": "admin",
    "modifiedBy": "admin"
  }
]
```

#### Create Login

```http
POST /api/SqlLogin
Content-Type: application/json

{
  "loginName": "newuser",
  "password": "SecurePass123!",
  "loginType": "SQL",
  "serverInstance": "localhost",
  "defaultDatabase": "master"
}
```

#### Update Login

```http
PUT /api/SqlLogin/{loginName}/server/{serverInstance}
Content-Type: application/json

{
  "isEnabled": true,
  "newPassword": "NewSecurePass123!"
}
```

#### Delete Login

```http
DELETE /api/SqlLogin/{loginName}/server/{serverInstance}
```

#### Bulk Operations

```http
POST /api/SqlLogin/bulk
Content-Type: application/json

{
  "operation": "disable",
  "serverInstance": "localhost",
  "loginNames": ["user1", "user2", "user3"]
}
```

### Server Role Management

#### Get All Server Roles

```http
GET /api/ServerRole/server/{serverInstance}
```

#### Create Server Role

```http
POST /api/ServerRole
Content-Type: application/json

{
  "roleName": "CustomRole",
  "serverInstance": "localhost"
}
```

#### Role Membership

```http
POST /api/ServerRole/members
Content-Type: application/json

{
  "roleName": "CustomRole",
  "memberName": "testuser",
  "serverInstance": "localhost"
}
```

### Database User Management

#### Get Database Users

```http
GET /api/DatabaseUser/database/{databaseName}/server/{serverInstance}
```

#### Create Database User

```http
POST /api/DatabaseUser
Content-Type: application/json

{
  "userName": "dbuser",
  "loginName": "testuser",
  "databaseName": "TestDB",
  "serverInstance": "localhost",
  "defaultSchema": "dbo"
}
```

#### User Role Assignment

```http
POST /api/DatabaseUser/roles
Content-Type: application/json

{
  "memberName": "dbuser",
  "roleName": "db_datareader",
  "databaseName": "TestDB",
  "serverInstance": "localhost"
}
```

### Database Role Management

#### Get Database Roles

```http
GET /api/DatabaseRole/database/{databaseName}/server/{serverInstance}
```

#### Create Database Role

```http
POST /api/DatabaseRole
Content-Type: application/json

{
  "roleName": "CustomDBRole",
  "databaseName": "TestDB",
  "serverInstance": "localhost"
}
```

## Usage Examples

### PowerShell Scripts

#### Create a Complete User Setup

```powershell
# Variables
$baseUrl = "https://localhost:7020/api"
$serverInstance = "localhost"
$databaseName = "TestDB"
$loginName = "testuser"
$password = "SecurePass123!"

# Create SQL Login
$loginBody = @{
    loginName = $loginName
    password = $password
    loginType = "SQL"
    serverInstance = $serverInstance
    defaultDatabase = $databaseName
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/SqlLogin" -Method POST -Body $loginBody -ContentType "application/json"

# Create Database User
$userBody = @{
    userName = $loginName
    loginName = $loginName
    databaseName = $databaseName
    serverInstance = $serverInstance
    defaultSchema = "dbo"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/DatabaseUser" -Method POST -Body $userBody -ContentType "application/json"

# Add User to Role
$roleBody = @{
    memberName = $loginName
    roleName = "db_datareader"
    databaseName = $databaseName
    serverInstance = $serverInstance
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/DatabaseUser/roles" -Method POST -Body $roleBody -ContentType "application/json"
```

### cURL Scripts

#### Create Login

```bash
curl -X POST "https://localhost:7020/api/SqlLogin" \
  -H "Content-Type: application/json" \
  -d '{
    "loginName": "testuser",
    "password": "SecurePass123!",
    "loginType": "SQL",
    "serverInstance": "localhost",
    "defaultDatabase": "master"
  }'
```

#### Get All Logins

```bash
curl -X GET "https://localhost:7020/api/SqlLogin/server/localhost"
```

#### Bulk Disable Logins

```bash
curl -X POST "https://localhost:7020/api/SqlLogin/bulk" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "disable",
    "serverInstance": "localhost",
    "loginNames": ["user1", "user2", "user3"]
  }'
```

## Testing

### Unit Tests

```bash
dotnet test
```

### Integration Tests

```bash
# Start test database
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=TestPass123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest

# Run integration tests
dotnet test --filter "Category=Integration"
```

### API Testing with Postman

Import the Postman collection from `docs/postman/` directory and run the test suite.

## Security

### Best Practices

1. **Authentication**: Implement proper authentication (JWT, OAuth2)
2. **Authorization**: Use role-based access control
3. **Input Validation**: All inputs are validated server-side
4. **SQL Injection Prevention**: Uses parameterized queries
5. **Password Policies**: Enforce strong password requirements
6. **Audit Logging**: All operations are logged
7. **HTTPS**: Always use HTTPS in production
8. **Connection Strings**: Store sensitive data in secure configuration

### Security Headers Configuration

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    await next();
});
```

### Data Protection

- Passwords are hashed using SQL Server's built-in functions
- Sensitive data is encrypted in transit and at rest
- Personal identifiable information is masked in logs

## Performance

### Optimization Tips

1. **Database Indexing**: Ensure proper indexes on frequently queried columns
2. **Connection Pooling**: Configure appropriate connection pool sizes
3. **Caching**: Implement Redis caching for frequently accessed data
4. **Bulk Operations**: Use bulk operations for multiple items
5. **Async Operations**: All database operations are asynchronous

### Performance Monitoring

```csharp
// Enable detailed logging for performance analysis
builder.Services.AddDbContext<SqlServerManagementDbContext>(options =>
{
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .EnableDetailedErrors(builder.Environment.IsDevelopment())
           .LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Information);
});
```

### Recommended Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SqlServerManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true;Connection Timeout=30;Command Timeout=300;Max Pool Size=100;"
  }
}
```

## Troubleshooting

### Common Issues

#### 1. Database Connection Issues

**Problem**: Cannot connect to SQL Server
**Solution**: 
- Check connection string
- Verify SQL Server is running
- Check firewall settings
- Ensure SQL Server authentication is enabled

#### 2. Migration Issues

**Problem**: Database migration fails
**Solution**:
```bash
# Reset migrations
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### 3. Permission Denied Errors

**Problem**: Access denied when creating logins/users
**Solution**: 
- Ensure the connection account has `securityadmin` or `sysadmin` permissions
- Check SQL Server security policies

#### 4. Temporal Connection Issues

**Problem**: Cannot connect to Temporal server
**Solution**:
- Verify Temporal server is running
- Check connection string in configuration
- Ensure network connectivity

### Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### Health Checks

The service includes built-in health checks:

```http
GET /health
```

Response:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "sqlserver": {
      "status": "Healthy",
      "duration": "00:00:00.0098765"
    }
  }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Write unit tests for new features
- Ensure all tests pass before submitting PR

### Development Setup

```bash
# Install development tools
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator

# Setup pre-commit hooks
git config core.hooksPath .githooks
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support and questions:
- Open an issue on GitHub
- Contact the development team
- Check the troubleshooting guide above

---

**Note**: This service is part of the larger IAMBuddy ecosystem. For complete setup and integration instructions, refer to the main IAMBuddy documentation.