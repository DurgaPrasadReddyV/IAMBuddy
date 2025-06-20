# SQL Server Management Service API Documentation

## Overview

The SQL Server Management Service provides RESTful APIs for comprehensive SQL Server identity and access management. This document provides detailed information about all available endpoints, request/response formats, and usage examples.

## Base URL

```
https://localhost:7020/api
```

## Authentication

Currently runs without authentication for development. In production, implement JWT Bearer authentication:

```http
Authorization: Bearer <your-jwt-token>
```

## Response Format

All responses follow a consistent format:

### Success Response
```json
{
  "data": { ... },
  "status": "success",
  "message": "Operation completed successfully"
}
```

### Error Response
```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human readable error message",
    "details": { ... }
  },
  "status": "error"
}
```

## SQL Login Management

### 1. Get All Logins

Retrieves all SQL Server logins for a specific server instance.

**Endpoint:**
```http
GET /api/SqlLogin/server/{serverInstance}
```

**Parameters:**
- `serverInstance` (path, required): The SQL Server instance name

**Example Request:**
```bash
curl -X GET "https://localhost:7020/api/SqlLogin/server/localhost"
```

**Example Response:**
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
  },
  {
    "id": 2,
    "loginName": "DOMAIN\\windowsuser",
    "loginType": "Windows",
    "sid": "0xFEDCBA987654321",
    "isEnabled": true,
    "isLocked": false,
    "passwordExpiryDate": null,
    "lastLoginDate": "2024-01-14T14:22:00Z",
    "serverInstance": "localhost",
    "createdDate": "2024-01-01T09:00:00Z",
    "modifiedDate": "2024-01-14T14:22:00Z",
    "createdBy": "admin",
    "modifiedBy": "admin"
  }
]
```

**Status Codes:**
- `200 OK`: Successfully retrieved logins
- `400 Bad Request`: Invalid server instance
- `500 Internal Server Error`: Server error

### 2. Get Specific Login

Retrieves a specific SQL Server login by name.

**Endpoint:**
```http
GET /api/SqlLogin/{loginName}/server/{serverInstance}
```

**Parameters:**
- `loginName` (path, required): The login name
- `serverInstance` (path, required): The SQL Server instance name

**Example Request:**
```bash
curl -X GET "https://localhost:7020/api/SqlLogin/testuser/server/localhost"
```

**Example Response:**
```json
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
```

**Status Codes:**
- `200 OK`: Login found and returned
- `404 Not Found`: Login not found
- `400 Bad Request`: Invalid parameters

### 3. Create Login

Creates a new SQL Server login.

**Endpoint:**
```http
POST /api/SqlLogin
```

**Request Body:**
```json
{
  "loginName": "newuser",
  "password": "SecurePass123!",
  "loginType": "SQL",
  "serverInstance": "localhost",
  "defaultDatabase": "master"
}
```

**Request Schema:**
- `loginName` (string, required): Login name (1-128 characters)
- `password` (string, optional): Password for SQL logins
- `loginType` (string, required): "SQL" or "Windows"
- `serverInstance` (string, required): Target server instance
- `defaultDatabase` (string, optional): Default database name

**Example Request:**
```bash
curl -X POST "https://localhost:7020/api/SqlLogin" \
  -H "Content-Type: application/json" \
  -d '{
    "loginName": "newuser",
    "password": "SecurePass123!",
    "loginType": "SQL",
    "serverInstance": "localhost",
    "defaultDatabase": "master"
  }'
```

**Example Response:**
```json
{
  "id": 3,
  "loginName": "newuser",
  "loginType": "SQL",
  "sid": "0x1122334455667788",
  "isEnabled": true,
  "isLocked": false,
  "passwordExpiryDate": null,
  "lastLoginDate": null,
  "serverInstance": "localhost",
  "createdDate": "2024-01-16T09:15:00Z",
  "modifiedDate": "2024-01-16T09:15:00Z",
  "createdBy": "admin",
  "modifiedBy": "admin"
}
```

**Status Codes:**
- `201 Created`: Login created successfully
- `400 Bad Request`: Invalid request data
- `409 Conflict`: Login already exists

### 4. Update Login

Updates an existing SQL Server login.

**Endpoint:**
```http
PUT /api/SqlLogin/{loginName}/server/{serverInstance}
```

**Request Body:**
```json
{
  "isEnabled": false,
  "newPassword": "NewSecurePass123!"
}
```

**Request Schema:**
- `isEnabled` (boolean, optional): Enable/disable the login
- `newPassword` (string, optional): New password for the login

**Example Request:**
```bash
curl -X PUT "https://localhost:7020/api/SqlLogin/testuser/server/localhost" \
  -H "Content-Type: application/json" \
  -d '{
    "isEnabled": false,
    "newPassword": "NewSecurePass123!"
  }'
```

**Status Codes:**
- `204 No Content`: Login updated successfully
- `400 Bad Request`: Invalid request data
- `404 Not Found`: Login not found

### 5. Delete Login

Deletes a SQL Server login.

**Endpoint:**
```http
DELETE /api/SqlLogin/{loginName}/server/{serverInstance}
```

**Example Request:**
```bash
curl -X DELETE "https://localhost:7020/api/SqlLogin/testuser/server/localhost"
```

**Status Codes:**
- `204 No Content`: Login deleted successfully
- `400 Bad Request`: Invalid parameters
- `404 Not Found`: Login not found

### 6. Get Login Server Roles

Retrieves all server roles assigned to a login.

**Endpoint:**
```http
GET /api/SqlLogin/{loginName}/server/{serverInstance}/roles
```

**Example Request:**
```bash
curl -X GET "https://localhost:7020/api/SqlLogin/testuser/server/localhost/roles"
```

**Example Response:**
```json
[
  "public",
  "db_datareader",
  "custom_role"
]
```

### 7. Add Login to Server Role

Adds a login to a server role.

**Endpoint:**
```http
POST /api/SqlLogin/{loginName}/server/{serverInstance}/roles/{roleName}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7020/api/SqlLogin/testuser/server/localhost/roles/db_datareader"
```

**Status Codes:**
- `204 No Content`: Login added to role successfully
- `400 Bad Request`: Invalid parameters
- `404 Not Found`: Login or role not found

### 8. Remove Login from Server Role

Removes a login from a server role.

**Endpoint:**
```http
DELETE /api/SqlLogin/{loginName}/server/{serverInstance}/roles/{roleName}
```

**Example Request:**
```bash
curl -X DELETE "https://localhost:7020/api/SqlLogin/testuser/server/localhost/roles/db_datareader"
```

### 9. Bulk Login Operations

Performs bulk operations on multiple logins.

**Endpoint:**
```http
POST /api/SqlLogin/bulk
```

**Request Body:**
```json
{
  "operation": "disable",
  "serverInstance": "localhost",
  "loginNames": ["user1", "user2", "user3"]
}
```

**Request Schema:**
- `operation` (string, required): "enable", "disable", or "delete"
- `serverInstance` (string, required): Target server instance
- `loginNames` (array, required): List of login names

**Example Response:**
```json
{
  "totalItems": 3,
  "successCount": 2,
  "failureCount": 1,
  "successfulItems": ["user1", "user2"],
  "failedItems": {
    "user3": "Login not found"
  }
}
```

## Server Role Management

### 1. Get All Server Roles

Retrieves all server roles for a specific server instance.

**Endpoint:**
```http
GET /api/ServerRole/server/{serverInstance}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7020/api/ServerRole/server/localhost"
```

**Example Response:**
```json
[
  {
    "id": 1,
    "roleName": "sysadmin",
    "roleType": "ServerRole",
    "databaseName": null,
    "serverInstance": "localhost",
    "description": "System administrators",
    "isBuiltIn": true,
    "isEnabled": true,
    "createdDate": "2024-01-01T09:00:00Z",
    "modifiedDate": "2024-01-01T09:00:00Z",
    "createdBy": "system",
    "modifiedBy": "system"
  }
]
```

### 2. Create Server Role

Creates a new server role.

**Endpoint:**
```http
POST /api/ServerRole
```

**Request Body:**
```json
{
  "roleName": "CustomServerRole",
  "serverInstance": "localhost"
}
```

**Example Request:**
```bash
curl -X POST "https://localhost:7020/api/ServerRole" \
  -H "Content-Type: application/json" \
  -d '{
    "roleName": "CustomServerRole",
    "serverInstance": "localhost"
  }'
```

### 3. Get Role Members

Gets all members of a server role.

**Endpoint:**
```http
GET /api/ServerRole/{roleName}/server/{serverInstance}/members
```

**Example Response:**
```json
[
  "sa",
  "testuser",
  "DOMAIN\\adminuser"
]
```

### 4. Add Role Member

Adds a member to a server role.

**Endpoint:**
```http
POST /api/ServerRole/members
```

**Request Body:**
```json
{
  "roleName": "CustomServerRole",
  "memberName": "testuser",
  "serverInstance": "localhost"
}
```

## Database User Management

### 1. Get Database Users

Retrieves all users for a specific database.

**Endpoint:**
```http
GET /api/DatabaseUser/database/{databaseName}/server/{serverInstance}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7020/api/DatabaseUser/database/TestDB/server/localhost"
```

**Example Response:**
```json
[
  {
    "id": 1,
    "userName": "dbuser",
    "databaseName": "TestDB",
    "serverInstance": "localhost",
    "sid": "0x123456789ABCDEF",
    "isEnabled": true,
    "userType": "SQL_USER",
    "defaultSchema": "dbo",
    "loginId": 1,
    "loginName": "testuser",
    "createdDate": "2024-01-01T09:00:00Z",
    "modifiedDate": "2024-01-01T09:00:00Z",
    "createdBy": "admin",
    "modifiedBy": "admin"
  }
]
```

### 2. Get Users by Login

Retrieves all database users associated with a specific login.

**Endpoint:**
```http
GET /api/DatabaseUser/login/{loginName}/server/{serverInstance}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7020/api/DatabaseUser/login/testuser/server/localhost"
```

### 3. Create Database User

Creates a new database user.

**Endpoint:**
```http
POST /api/DatabaseUser
```

**Request Body:**
```json
{
  "userName": "dbuser",
  "loginName": "testuser",
  "databaseName": "TestDB",
  "serverInstance": "localhost",
  "defaultSchema": "dbo"
}
```

**Request Schema:**
- `userName` (string, required): Database user name
- `loginName` (string, optional): Associated login name (if null, creates user without login)
- `databaseName` (string, required): Target database name
- `serverInstance` (string, required): Target server instance
- `defaultSchema` (string, optional): Default schema for the user

### 4. Update Database User

Updates an existing database user.

**Endpoint:**
```http
PUT /api/DatabaseUser/{userName}/database/{databaseName}/server/{serverInstance}
```

**Request Body:**
```json
{
  "defaultSchema": "sales"
}
```

### 5. Delete Database User

Deletes a database user.

**Endpoint:**
```http
DELETE /api/DatabaseUser/{userName}/database/{databaseName}/server/{serverInstance}
```

### 6. Get User Roles

Gets all database roles assigned to a user.

**Endpoint:**
```http
GET /api/DatabaseUser/{userName}/database/{databaseName}/server/{serverInstance}/roles
```

**Example Response:**
```json
[
  "db_datareader",
  "db_datawriter",
  "custom_db_role"
]
```

### 7. Add User to Role

Adds a user to a database role.

**Endpoint:**
```http
POST /api/DatabaseUser/roles
```

**Request Body:**
```json
{
  "memberName": "dbuser",
  "roleName": "db_datareader",
  "databaseName": "TestDB",
  "serverInstance": "localhost"
}
```

### 8. Remove User from Role

Removes a user from a database role.

**Endpoint:**
```http
DELETE /api/DatabaseUser/roles
```

**Request Body:**
```json
{
  "memberName": "dbuser",
  "roleName": "db_datareader",
  "databaseName": "TestDB",
  "serverInstance": "localhost"
}
```

## Database Role Management

### 1. Get Database Roles

Retrieves all roles for a specific database.

**Endpoint:**
```http
GET /api/DatabaseRole/database/{databaseName}/server/{serverInstance}
```

**Example Request:**
```bash
curl -X GET "https://localhost:7020/api/DatabaseRole/database/TestDB/server/localhost"
```

**Example Response:**
```json
[
  {
    "id": 1,
    "roleName": "db_datareader",
    "roleType": "DatabaseRole",
    "databaseName": "TestDB",
    "serverInstance": "localhost",
    "description": "Database data reader role",
    "isBuiltIn": true,
    "isEnabled": true,
    "createdDate": "2024-01-01T09:00:00Z",
    "modifiedDate": "2024-01-01T09:00:00Z",
    "createdBy": "system",
    "modifiedBy": "system"
  }
]
```

### 2. Create Database Role

Creates a new database role.

**Endpoint:**
```http
POST /api/DatabaseRole
```

**Request Body:**
```json
{
  "roleName": "CustomDBRole",
  "databaseName": "TestDB",
  "serverInstance": "localhost"
}
```

### 3. Get Role Members

Gets all members of a database role.

**Endpoint:**
```http
GET /api/DatabaseRole/{roleName}/database/{databaseName}/server/{serverInstance}/members
```

**Example Response:**
```json
[
  "dbuser1",
  "dbuser2",
  "reportuser"
]
```

### 4. Role Permission Management

#### Grant Permission

**Endpoint:**
```http
POST /api/DatabaseRole/permissions
```

**Request Body:**
```json
{
  "roleName": "CustomDBRole",
  "permission": "SELECT",
  "databaseName": "TestDB",
  "serverInstance": "localhost",
  "objectName": "Users"
}
```

#### Revoke Permission

**Endpoint:**
```http
DELETE /api/DatabaseRole/permissions
```

**Request Body:**
```json
{
  "roleName": "CustomDBRole",
  "permission": "SELECT",
  "databaseName": "TestDB",
  "serverInstance": "localhost",
  "objectName": "Users"
}
```

## Error Handling

### Common Error Responses

#### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "0HN7MGUKSG9MQ:00000001",
  "errors": {
    "LoginName": [
      "The LoginName field is required."
    ],
    "Password": [
      "Password must contain at least one uppercase letter."
    ]
  }
}
```

#### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Login 'nonexistentuser' not found on server 'localhost'",
  "traceId": "0HN7MGUKSG9MQ:00000002"
}
```

#### 409 Conflict
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Login 'testuser' already exists on server 'localhost'",
  "traceId": "0HN7MGUKSG9MQ:00000003"
}
```

#### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "traceId": "0HN7MGUKSG9MQ:00000004"
}
```

## Rate Limiting

The API implements rate limiting to prevent abuse:

- **Rate Limit**: 100 requests per minute per IP
- **Headers**:
  - `X-RateLimit-Limit`: Maximum requests per window
  - `X-RateLimit-Remaining`: Remaining requests in current window
  - `X-RateLimit-Reset`: Unix timestamp when the window resets

## Pagination

For endpoints that return collections, pagination is supported:

**Query Parameters:**
- `page` (int, default: 1): Page number
- `pageSize` (int, default: 50, max: 100): Items per page

**Response Headers:**
- `X-Pagination-CurrentPage`: Current page number
- `X-Pagination-TotalPages`: Total number of pages
- `X-Pagination-TotalCount`: Total number of items

**Example:**
```http
GET /api/SqlLogin/server/localhost?page=2&pageSize=25
```

## OpenAPI/Swagger Documentation

Interactive API documentation is available at:
- Development: `https://localhost:7020/swagger`
- OpenAPI JSON: `https://localhost:7020/swagger/v1/swagger.json`

## SDK and Client Libraries

### .NET Client Example

```csharp
using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://localhost:7020/");

var loginRequest = new CreateSqlLoginRequest
{
    LoginName = "testuser",
    Password = "SecurePass123!",
    LoginType = "SQL",
    ServerInstance = "localhost",
    DefaultDatabase = "master"
};

var json = JsonSerializer.Serialize(loginRequest);
var content = new StringContent(json, Encoding.UTF8, "application/json");

var response = await httpClient.PostAsync("api/SqlLogin", content);
if (response.IsSuccessStatusCode)
{
    var createdLogin = await response.Content.ReadFromJsonAsync<SqlLoginDto>();
    Console.WriteLine($"Created login: {createdLogin.LoginName}");
}
```

### JavaScript/TypeScript Client Example

```typescript
interface CreateSqlLoginRequest {
  loginName: string;
  password?: string;
  loginType: 'SQL' | 'Windows';
  serverInstance: string;
  defaultDatabase?: string;
}

class SqlServerManagementClient {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  async createLogin(request: CreateSqlLoginRequest): Promise<SqlLoginDto> {
    const response = await fetch(`${this.baseUrl}/api/SqlLogin`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
  }
}
```

## Testing and Validation

### Health Check Endpoint

**Endpoint:**
```http
GET /health
```

**Example Response:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "sqlserver": {
      "status": "Healthy",
      "duration": "00:00:00.0098765",
      "data": {
        "connectionString": "Server=localhost;Database=SqlServerManagementDb;..."
      }
    }
  }
}
```

### Readiness and Liveness Probes

**Liveness Probe:**
```http
GET /health/live
```

**Readiness Probe:**
```http
GET /health/ready
```

These endpoints are designed for Kubernetes health checks and return 200 OK when the service is healthy.

---

This API documentation provides comprehensive coverage of all available endpoints. For additional information or support, please refer to the main README.md file or contact the development team.