# IAMBuddy Database Setup and Migration Guide

## Overview

This document provides comprehensive information about the Entity Framework Core migrations, database configuration, and deployment procedures for the IAMBuddy system.

## Database Architecture

The IAMBuddy system uses two separate databases:

1. **IAMBuddy_RequestIntake** - Stores MSSQL account requests and workflow state
2. **IAMBuddy_SqlServerManagement** - Stores SQL Server logins, roles, users, and operations

## Database Features Implemented

### 1. Entity Framework Core Configuration
- **RequestIntakeService**: Updated to use SQL Server with EF Core 9.0
- **SqlServerManagementService**: Already configured with SQL Server
- Migration tools and design packages added to both services

### 2. Initial Migrations Created
- **RequestIntakeService**: `InitialCreate` migration for MSSQLAccountRequest model
- **SqlServerManagementService**: `InitialCreate` migration for all SQL Server management models

### 3. Performance Indexes Added
- **AccountRequests Table**:
  - `IX_AccountRequests_Status` - For filtering by request status
  - `IX_AccountRequests_RequestedDate` - For date-based queries
  - `IX_AccountRequests_RequestorEmail` - For user-specific queries
  - `IX_AccountRequests_WorkflowId` - For workflow tracking
  - `IX_AccountRequests_Username_Server_Database` - Composite index for unique constraint

### 4. Database Seeding
- **Default SQL Server Roles**: Pre-populated with all standard server and database roles
  - Server roles: sysadmin, serveradmin, securityadmin, processadmin, setupadmin, bulkadmin, diskadmin, dbcreator, public
  - Database roles: db_owner, db_accessadmin, db_securityadmin, db_ddladmin, db_backupoperator, db_datareader, db_datawriter, db_denydatareader, db_denydatawriter

### 5. Connection String Configuration
- **Development**: Uses LocalDB with separate databases
- **Production**: Environment variable-based configuration
- EF Core logging enabled for database commands

### 6. Database Initialization
- **Automatic Migration**: Both services run migrations on startup
- **Error Handling**: Comprehensive logging and error handling
- **Environment-aware**: Different behaviors for development vs production

### 7. Independent Migration Scripts
- **SQL Scripts**: Generated for both services in `Scripts/InitialCreate.sql`
- **PowerShell Script**: `deploy-database.ps1` for Windows deployment
- **Bash Script**: `deploy-database.sh` for Unix/Linux deployment

### 8. .NET Aspire Integration
- **AppHost Configuration**: SQL Server container with persistent volumes
- **Database References**: Services automatically receive connection strings
- **Service Dependencies**: Proper startup order with database dependencies

## Connection Strings

### Development (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IAMBuddy_RequestIntake;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Production (Environment Variables)
- `IAMBUDDY_REQUESTINTAKE_CONNECTION_STRING`
- `IAMBUDDY_SQLSERVERMANAGEMENT_CONNECTION_STRING`

## Migration Commands

### Generate New Migration
```bash
# RequestIntakeService
cd IAMBuddy.RequestIntakeService
dotnet ef migrations add <MigrationName>

# SqlServerManagementService
cd IAMBuddy.SqlServerManagementService
dotnet ef migrations add <MigrationName>
```

### Generate SQL Scripts
```bash
# RequestIntakeService
cd IAMBuddy.RequestIntakeService
dotnet ef migrations script --output Scripts/<ScriptName>.sql

# SqlServerManagementService
cd IAMBuddy.SqlServerManagementService
dotnet ef migrations script --output Scripts/<ScriptName>.sql
```

### Manual Database Update
```bash
# RequestIntakeService
cd IAMBuddy.RequestIntakeService
dotnet ef database update

# SqlServerManagementService
cd IAMBuddy.SqlServerManagementService
dotnet ef database update
```

## Deployment Scripts

### PowerShell (Windows)
```powershell
# Deploy all services
./deploy-database.ps1 -Environment Production

# Deploy specific service
./deploy-database.ps1 -Environment Production -RequestIntakeOnly

# Dry run
./deploy-database.ps1 -Environment Production -WhatIf
```

### Bash (Unix/Linux)
```bash
# Deploy all services
./deploy-database.sh Production

# Deploy specific service
./deploy-database.sh Production true false

# Dry run
./deploy-database.sh Production false false true
```

## EF Core Features Configured

### Logging and Monitoring
- **Sensitive Data Logging**: Enabled in development only
- **Detailed Errors**: Enabled in development only
- **SQL Command Logging**: Configured at Information level
- **Debug Output**: SQL queries logged to debug output

### Performance Features
- **Connection Pooling**: Automatic with SQL Server provider
- **Query Caching**: EF Core built-in query plan caching
- **Indexes**: Strategic indexes for common query patterns
- **Audit Fields**: Automatic CreatedDate/ModifiedDate updates

### Database Constraints
- **Foreign Keys**: Proper relationships with cascade behaviors
- **Check Constraints**: Role assignment validation
- **Unique Indexes**: Prevent duplicate entries
- **Required Fields**: Null validation at database level

## Production Considerations

### Security
- Connection strings should use Azure Key Vault or similar secret management
- Sensitive data logging must be disabled in production
- Use managed identity for Azure SQL Database connections

### Performance
- Consider implementing read replicas for heavy read workloads
- Monitor query performance using Application Insights
- Implement proper connection pooling configuration
- Consider table partitioning for large datasets

### Backup and Recovery
- Implement automated backup strategies
- Test disaster recovery procedures
- Consider point-in-time recovery requirements
- Document backup retention policies

## Troubleshooting

### Common Issues
1. **Migration Failures**: Check connection strings and permissions
2. **Seed Data Issues**: Verify unique constraints and data integrity
3. **Performance Problems**: Review indexes and query execution plans
4. **Connection Issues**: Validate network connectivity and authentication

### Debug Commands
```bash
# Check migration status
dotnet ef migrations list

# Generate migration bundle
dotnet ef migrations bundle

# View SQL for specific migration
dotnet ef migrations script <FromMigration> <ToMigration>
```

## Next Steps

1. **Monitoring**: Implement database performance monitoring
2. **Backup Strategy**: Configure automated backups
3. **Security**: Implement proper authentication and authorization
4. **Testing**: Create integration tests for database operations
5. **Documentation**: Update API documentation with database schema changes