# IAMBuddy MSSQL Account Manager - Architecture Documentation

## System Architecture Overview

The IAMBuddy MSSQL Account Manager follows a clean architecture pattern with clear separation of concerns, implementing enterprise-grade patterns for maintainability, testability, and scalability.

## Design Principles

### SOLID Principles Implementation

#### Single Responsibility Principle (SRP)
- **Controllers**: Handle only HTTP request/response mapping
- **Services**: Contain business logic for specific domains
- **Models**: Represent data structures and validation rules
- **Data Layer**: Manages data persistence and retrieval

#### Open/Closed Principle (OCP)
- **Service Interfaces**: Allow extension without modification
- **Generic Base Classes**: Enable reusable functionality
- **Configuration-based behavior**: Extensible through settings

#### Liskov Substitution Principle (LSP)
- **Service Implementations**: All implementations honor their contracts
- **Repository Pattern**: Consistent data access interface

#### Interface Segregation Principle (ISP)
- **Fine-grained Interfaces**: Each service interface focuses on specific functionality
- **No Fat Interfaces**: Clients depend only on methods they use

#### Dependency Inversion Principle (DIP)
- **Dependency Injection**: All dependencies injected via constructor
- **Interface Dependencies**: Depend on abstractions, not concrete implementations

### DRY (Don't Repeat Yourself)
- **Base Entity Class**: Common audit fields
- **Generic Operations**: Reusable CRUD patterns
- **Shared Utilities**: Connection string management

## Architectural Layers

### 1. Presentation Layer (Controllers)

```
Controllers/
├── SqlLoginController.cs        # SQL login management endpoints
├── ServerRoleController.cs      # Server role management endpoints
├── DatabaseUserController.cs    # Database user management endpoints
├── DatabaseRoleController.cs    # Database role management endpoints
├── ServerDiscoveryController.cs # Server instance management endpoints
└── AuditController.cs          # Audit trail endpoints
```

**Responsibilities:**
- HTTP request/response handling
- Input validation and model binding
- Error response formatting
- API documentation attributes

**Design Decisions:**
- RESTful API design with standard HTTP verbs
- Consistent response patterns
- Comprehensive error handling
- OpenAPI/Swagger documentation

### 2. Business Logic Layer (Services)

```
Services/
├── Interfaces/
│   ├── ISqlLoginService.cs      # SQL login operations contract
│   ├── IServerRoleService.cs    # Server role operations contract
│   ├── IDatabaseUserService.cs  # Database user operations contract
│   ├── IDatabaseRoleService.cs  # Database role operations contract
│   ├── IServerDiscoveryService.cs # Server discovery contract
│   └── IAuditService.cs         # Audit operations contract
├── SqlLoginService.cs           # SQL login business logic
├── ServerRoleService.cs         # Server role business logic
├── DatabaseUserService.cs      # Database user business logic
├── DatabaseRoleService.cs      # Database role business logic
├── ServerDiscoveryService.cs   # Server discovery business logic
└── AuditService.cs             # Audit trail business logic
```

**Responsibilities:**
- Business rule enforcement
- Transaction management
- External system coordination (SQL Server operations)
- Audit trail generation

**Design Decisions:**
- Interface-based design for testability
- Async/await patterns for I/O operations
- Comprehensive error handling and logging
- Transactional consistency for multi-step operations

### 3. Data Access Layer

```
Data/
└── MSSQLAccountManagerContext.cs  # Entity Framework DbContext

Models/
├── BaseEntity.cs               # Common audit fields
├── SqlLogin.cs                # SQL login entity
├── ServerRole.cs              # Server role entity
├── DatabaseUser.cs            # Database user entity
├── DatabaseRole.cs            # Database role entity
├── ServerInstance.cs          # Server instance entity
├── OperationResult.cs         # Audit trail entity
├── ServerRoleAssignment.cs    # Login-role relationship
└── DatabaseRoleAssignment.cs  # User-role relationship
```

**Responsibilities:**
- Data persistence and retrieval
- Relationship management
- Query optimization
- Migration support

**Design Decisions:**
- Entity Framework Core for application data
- Dapper for SQL Server management operations
- Code-First database approach
- Explicit relationship configuration

## Data Architecture

### Entity Relationship Diagram

```
┌─────────────┐    ┌─────────────────────┐    ┌─────────────┐
│ SqlLogin    │────│ ServerRoleAssignment │────│ ServerRole  │
└─────────────┘    └─────────────────────┘    └─────────────┘
       │                                              
       │ 1:N                                          
       ▼                                              
┌─────────────┐    ┌──────────────────────┐    ┌─────────────┐
│DatabaseUser │────│DatabaseRoleAssignment│────│DatabaseRole │
└─────────────┘    └──────────────────────┘    └─────────────┘

┌─────────────┐
│ServerInstance│
└─────────────┘

┌─────────────┐
│OperationResult│ (Audit Trail)
└─────────────┘
```

### Key Relationships

1. **SqlLogin → DatabaseUser**: One-to-Many
   - A login can map to multiple database users across different databases

2. **SqlLogin → ServerRoleAssignment**: One-to-Many
   - A login can have multiple server role assignments

3. **DatabaseUser → DatabaseRoleAssignment**: One-to-Many
   - A database user can have multiple database role assignments

4. **Audit Trail**: Independent tracking of all operations

### Database Design Decisions

#### Normalization
- **3rd Normal Form**: Eliminates data redundancy
- **Separate Assignment Tables**: Maintains temporal data for role assignments
- **Audit Trail**: Complete operation tracking with success/failure status

#### Indexing Strategy
- **Unique Constraints**: Prevent duplicate resources per server
- **Foreign Key Indexes**: Optimize relationship queries
- **Composite Indexes**: Server + Instance + Resource name lookups

#### Data Types
- **GUIDs**: Primary keys for distributed system compatibility
- **UTC Timestamps**: Consistent time tracking across time zones
- **Enums as Strings**: Readable and maintainable status values

## Security Architecture

### Authentication & Authorization

```
┌─────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Client    │───▶│   API Gateway   │───▶│ Account Manager │
└─────────────┘    └─────────────────┘    └─────────────────┘
                           │                        │
                           ▼                        ▼
                   ┌─────────────────┐    ┌─────────────────┐
                   │  Identity       │    │  SQL Server     │
                   │  Provider       │    │  (Target)       │
                   └─────────────────┘    └─────────────────┘
```

### Security Layers

1. **Network Security**
   - TLS encryption for all communications
   - Network isolation via Docker networking
   - Firewall rules for port access

2. **Application Security**
   - Input validation using FluentValidation
   - SQL injection prevention via parameterized queries
   - Error message sanitization

3. **Data Security**
   - Connection string encryption
   - Audit trail for all operations
   - Sensitive data handling policies

### Permission Model

```
Service Account (MSSQL Account Manager)
├── Application Database: Full Control
└── Target SQL Servers: sysadmin (for server management)
    ├── Create/Alter/Drop Logins
    ├── Create/Alter/Drop Server Roles
    ├── Create/Alter/Drop Database Users
    └── Create/Alter/Drop Database Roles
```

## Technology Stack

### Core Technologies

| Component | Technology | Version | Purpose |
|-----------|------------|---------|---------|
| **Runtime** | .NET Core | 9.0 | Application platform |
| **Framework** | ASP.NET Core | 9.0 | Web API framework |
| **ORM (App Data)** | Entity Framework Core | 9.0 | Application data persistence |
| **ORM (SQL Ops)** | Dapper | 2.1.35 | High-performance SQL operations |
| **Database** | SQL Server | 2019+ | Target database platform |
| **Logging** | Serilog | 8.0.2 | Structured logging |
| **Validation** | FluentValidation | 11.3.0 | Input validation |
| **Documentation** | OpenAPI/Swagger | Built-in | API documentation |
| **Containerization** | Docker | 20.0+ | Deployment platform |

### Design Decisions Rationale

#### Entity Framework + Dapper Hybrid Approach
- **Entity Framework**: Application data management with complex relationships
- **Dapper**: High-performance SQL Server operations with precise control
- **Benefits**: Best of both worlds - productivity and performance

#### Serilog for Logging
- **Structured Logging**: JSON format for log analysis
- **Multiple Sinks**: Console and file output
- **Performance**: Asynchronous logging with minimal overhead

#### Docker Containerization
- **Consistency**: Same environment across development and production
- **Scalability**: Container orchestration support
- **Isolation**: Network and resource isolation

## Performance Architecture

### Optimization Strategies

#### Database Performance
- **Connection Pooling**: Automatic via SqlClient
- **Async Operations**: Non-blocking I/O for all database operations
- **Query Optimization**: Optimized Entity Framework queries
- **Indexes**: Strategic indexing for common query patterns

#### Memory Management
- **Dependency Injection**: Scoped lifetimes for services
- **Disposal Pattern**: Proper resource cleanup
- **Streaming**: Large result set handling

#### Caching Strategy
- **Connection String Caching**: In-memory caching for server instances
- **Metadata Caching**: Role and permission information
- **Response Caching**: Configurable for read-heavy operations

### Performance Metrics

Based on integration testing:

| Operation Type | Average Response Time | Notes |
|----------------|----------------------|-------|
| Server Registration | 400-500ms | Includes connection test |
| Login Creation | 250-350ms | Includes SQL Server DDL |
| Role Operations | 80-120ms | Fast DDL operations |
| User Creation | 80-100ms | Database-scoped operations |
| Audit Queries | <50ms | Optimized read operations |

## Scalability Architecture

### Horizontal Scaling
- **Stateless Design**: No session state in application
- **Database Per Service**: Independent scaling of data layer
- **Load Balancer Ready**: Standard HTTP API suitable for load balancing

### Vertical Scaling
- **Resource Optimization**: Efficient memory and CPU usage
- **Connection Management**: Optimized database connection pooling
- **Async Patterns**: Non-blocking operations for higher throughput

### Data Scaling
- **Partitioning Strategy**: Server-based data partitioning
- **Read Replicas**: Support for read-only database replicas
- **Audit Data Archival**: Strategies for large audit datasets

## Error Handling Architecture

### Exception Hierarchy

```
Application Exceptions
├── ValidationException (FluentValidation)
├── SqlException (Microsoft.Data.SqlClient)
├── EntityFrameworkException
└── BusinessLogicException (Custom)
```

### Error Response Pattern

```json
{
  "operationType": 0,
  "status": 1,
  "resourceType": "SqlLogin",
  "resourceName": "testuser",
  "serverName": "sqlserver",
  "errorMessage": "User-friendly error message",
  "details": "Technical details for debugging",
  "startTime": "2025-06-25T16:42:54.266Z",
  "endTime": "2025-06-25T16:42:54.383Z",
  "duration": 116
}
```

### Error Handling Strategy
- **Graceful Degradation**: Continue operation when possible
- **Detailed Logging**: Complete error context for debugging
- **User-Friendly Messages**: Clean error messages for API consumers
- **Operation Tracking**: All errors logged in audit trail

## Testing Architecture

### Testing Strategy

```
Testing Pyramid
├── Unit Tests (Services & Business Logic)
├── Integration Tests (Database & SQL Server)
├── API Tests (Controller Endpoints)
└── End-to-End Tests (Docker Compose)
```

### Test Categories

1. **Unit Tests**
   - Service layer business logic
   - Model validation
   - Utility functions

2. **Integration Tests**
   - Database operations
   - SQL Server connectivity
   - External service integration

3. **API Tests**
   - Controller endpoints
   - Request/response validation
   - Error handling

4. **Performance Tests**
   - Load testing
   - Stress testing
   - Memory usage profiling

## Deployment Architecture

### Container Strategy

```
Docker Multi-Stage Build
├── SDK Image (Build)
├── Runtime Image (Final)
└── Optimized Layers
```

### Environment Configuration

| Environment | Purpose | Configuration |
|-------------|---------|---------------|
| **Development** | Local development | LocalDB/SQL Express |
| **Testing** | Automated testing | SQL Server container |
| **Staging** | Pre-production testing | Azure SQL Database |
| **Production** | Live environment | SQL Server cluster |

### Monitoring & Observability

```
Observability Stack
├── Application Logs (Serilog)
├── Performance Metrics (Built-in)
├── Health Checks (ASP.NET Core)
└── Distributed Tracing (Ready)
```

## Future Architecture Considerations

### Microservices Evolution
- **Service Decomposition**: Further split by domain boundaries
- **Event-Driven Architecture**: Asynchronous communication between services
- **API Gateway**: Centralized routing and cross-cutting concerns

### Cloud-Native Features
- **Container Orchestration**: Kubernetes deployment
- **Service Mesh**: Advanced networking and security
- **Serverless Components**: Function-based operations

### Advanced Security
- **Zero Trust Architecture**: Continuous verification
- **Secret Management**: External secret stores
- **Certificate Management**: Automated certificate rotation

---

**Document Version**: 1.0  
**Last Updated**: June 25, 2025  
**Architecture Review Date**: June 25, 2025