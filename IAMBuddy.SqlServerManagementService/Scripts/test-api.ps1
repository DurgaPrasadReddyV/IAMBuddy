# SQL Server Management Service API Testing Script
# PowerShell script for testing all API endpoints

param(
    [string]$BaseUrl = "https://localhost:7020/api",
    [string]$ServerInstance = "localhost",
    [string]$DatabaseName = "TestDB",
    [switch]$SkipSslCheck
)

# Skip SSL certificate validation for testing (development only)
if ($SkipSslCheck) {
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
    if ($PSVersionTable.PSVersion.Major -ge 6) {
        $PSDefaultParameterValues['Invoke-RestMethod:SkipCertificateCheck'] = $true
        $PSDefaultParameterValues['Invoke-WebRequest:SkipCertificateCheck'] = $true
    }
}

Write-Host "Starting SQL Server Management Service API Tests" -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow
Write-Host "Server Instance: $ServerInstance" -ForegroundColor Yellow
Write-Host "Database: $DatabaseName" -ForegroundColor Yellow

# Test variables
$TestLoginName = "testuser_$(Get-Random)"
$TestPassword = "SecurePass123!"
$TestWindowsLogin = "DOMAIN\testuser_$(Get-Random)"
$TestServerRole = "CustomRole_$(Get-Random)"
$TestDatabaseRole = "CustomDBRole_$(Get-Random)"
$TestUserName = "dbuser_$(Get-Random)"

# Helper function for API calls
function Invoke-ApiCall {
    param(
        [string]$Method,
        [string]$Uri,
        [object]$Body,
        [string]$Description
    )
    
    Write-Host "`n--- $Description ---" -ForegroundColor Cyan
    Write-Host "$Method $Uri" -ForegroundColor Gray
    
    try {
        $params = @{
            Uri = $Uri
            Method = $Method
            ContentType = "application/json"
        }
        
        if ($Body) {
            $params.Body = $Body | ConvertTo-Json -Depth 10
            Write-Host "Request Body: $($params.Body)" -ForegroundColor Gray
        }
        
        $response = Invoke-RestMethod @params
        
        Write-Host "✅ SUCCESS" -ForegroundColor Green
        if ($response) {
            Write-Host "Response: $($response | ConvertTo-Json -Depth 2)" -ForegroundColor White
        }
        
        return $response
    }
    catch {
        Write-Host "❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails) {
            Write-Host "Error Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        return $null
    }
}

# Test 1: Health Check
Write-Host "`n🔍 Testing Health Check..." -ForegroundColor Magenta
Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/../health" -Description "Health Check"

# Test 2: Get All Logins (should work even if empty)
Write-Host "`n🔍 Testing Login Management..." -ForegroundColor Magenta
$logins = Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/SqlLogin/server/$ServerInstance" -Description "Get All Logins"

# Test 3: Create SQL Login
$createLoginRequest = @{
    loginName = $TestLoginName
    password = $TestPassword
    loginType = "SQL"
    serverInstance = $ServerInstance
    defaultDatabase = "master"
}

$createdLogin = Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/SqlLogin" -Body $createLoginRequest -Description "Create SQL Login"

if ($createdLogin) {
    # Test 4: Get Specific Login
    Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/SqlLogin/$TestLoginName/server/$ServerInstance" -Description "Get Specific Login"
    
    # Test 5: Update Login (disable)
    $updateRequest = @{
        isEnabled = $false
    }
    Invoke-ApiCall -Method "PUT" -Uri "$BaseUrl/SqlLogin/$TestLoginName/server/$ServerInstance" -Body $updateRequest -Description "Disable Login"
    
    # Test 6: Update Login (re-enable and change password)
    $updateRequest = @{
        isEnabled = $true
        newPassword = "NewSecurePass123!"
    }
    Invoke-ApiCall -Method "PUT" -Uri "$BaseUrl/SqlLogin/$TestLoginName/server/$ServerInstance" -Body $updateRequest -Description "Enable Login and Change Password"
    
    # Test 7: Get Login Server Roles
    Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/SqlLogin/$TestLoginName/server/$ServerInstance/roles" -Description "Get Login Server Roles"
}

# Test 8: Create Windows Login
$createWindowsLoginRequest = @{
    loginName = $TestWindowsLogin
    loginType = "Windows"
    serverInstance = $ServerInstance
    defaultDatabase = "master"
}

$createdWindowsLogin = Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/SqlLogin" -Body $createWindowsLoginRequest -Description "Create Windows Login"

# Test 9: Server Role Management
Write-Host "`n🔍 Testing Server Role Management..." -ForegroundColor Magenta

# Get all server roles
$serverRoles = Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/ServerRole/server/$ServerInstance" -Description "Get All Server Roles"

# Create custom server role
$createServerRoleRequest = @{
    roleName = $TestServerRole
    serverInstance = $ServerInstance
}

$createdServerRole = Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/ServerRole" -Body $createServerRoleRequest -Description "Create Server Role"

if ($createdServerRole) {
    # Get specific server role
    Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/ServerRole/$TestServerRole/server/$ServerInstance" -Description "Get Specific Server Role"
    
    # Get role members
    Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/ServerRole/$TestServerRole/server/$ServerInstance/members" -Description "Get Role Members"
    
    # Add member to role (if we have a login)
    if ($createdLogin) {
        $addMemberRequest = @{
            roleName = $TestServerRole
            memberName = $TestLoginName
            serverInstance = $ServerInstance
        }
        Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/ServerRole/members" -Body $addMemberRequest -Description "Add Member to Server Role"
        
        # Remove member from role
        Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/ServerRole/members" -Body $addMemberRequest -Description "Remove Member from Server Role"
    }
}

# Test 10: Database User Management
Write-Host "`n🔍 Testing Database User Management..." -ForegroundColor Magenta

# Get all database users
$databaseUsers = Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseUser/database/$DatabaseName/server/$ServerInstance" -Description "Get All Database Users"

# Create database user (with login)
if ($createdLogin) {
    $createUserRequest = @{
        userName = $TestUserName
        loginName = $TestLoginName
        databaseName = $DatabaseName
        serverInstance = $ServerInstance
        defaultSchema = "dbo"
    }
    
    $createdUser = Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/DatabaseUser" -Body $createUserRequest -Description "Create Database User with Login"
    
    if ($createdUser) {
        # Get specific database user
        Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseUser/$TestUserName/database/$DatabaseName/server/$ServerInstance" -Description "Get Specific Database User"
        
        # Get users by login
        Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseUser/login/$TestLoginName/server/$ServerInstance" -Description "Get Users by Login"
        
        # Update database user
        $updateUserRequest = @{
            defaultSchema = "sales"
        }
        Invoke-ApiCall -Method "PUT" -Uri "$BaseUrl/DatabaseUser/$TestUserName/database/$DatabaseName/server/$ServerInstance" -Body $updateUserRequest -Description "Update Database User"
        
        # Get user roles
        Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseUser/$TestUserName/database/$DatabaseName/server/$ServerInstance/roles" -Description "Get User Roles"
        
        # Add user to built-in role
        $addUserToRoleRequest = @{
            memberName = $TestUserName
            roleName = "db_datareader"
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
        }
        Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/DatabaseUser/roles" -Body $addUserToRoleRequest -Description "Add User to Database Role"
        
        # Remove user from role
        Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/DatabaseUser/roles" -Body $addUserToRoleRequest -Description "Remove User from Database Role"
    }
}

# Test 11: Database Role Management
Write-Host "`n🔍 Testing Database Role Management..." -ForegroundColor Magenta

# Get all database roles
$databaseRoles = Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseRole/database/$DatabaseName/server/$ServerInstance" -Description "Get All Database Roles"

# Create custom database role
$createDBRoleRequest = @{
    roleName = $TestDatabaseRole
    databaseName = $DatabaseName
    serverInstance = $ServerInstance
}

$createdDBRole = Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/DatabaseRole" -Body $createDBRoleRequest -Description "Create Database Role"

if ($createdDBRole) {
    # Get specific database role
    Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseRole/$TestDatabaseRole/database/$DatabaseName/server/$ServerInstance" -Description "Get Specific Database Role"
    
    # Get role members
    Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseRole/$TestDatabaseRole/database/$DatabaseName/server/$ServerInstance/members" -Description "Get Database Role Members"
    
    # Add member to database role (if we have a user)
    if ($createdUser) {
        $addDBRoleMemberRequest = @{
            roleName = $TestDatabaseRole
            memberName = $TestUserName
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
        }
        Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/DatabaseRole/members" -Body $addDBRoleMemberRequest -Description "Add Member to Database Role"
        
        # Remove member from database role
        Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/DatabaseRole/members" -Body $addDBRoleMemberRequest -Description "Remove Member from Database Role"
    }
    
    # Grant permission to role
    $grantPermissionRequest = @{
        roleName = $TestDatabaseRole
        permission = "SELECT"
        databaseName = $DatabaseName
        serverInstance = $ServerInstance
        objectName = "Users"
    }
    Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/DatabaseRole/permissions" -Body $grantPermissionRequest -Description "Grant Permission to Database Role"
    
    # Get role permissions
    Invoke-ApiCall -Method "GET" -Uri "$BaseUrl/DatabaseRole/$TestDatabaseRole/database/$DatabaseName/server/$ServerInstance/permissions" -Description "Get Database Role Permissions"
    
    # Revoke permission from role
    Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/DatabaseRole/permissions" -Body $grantPermissionRequest -Description "Revoke Permission from Database Role"
}

# Test 12: Bulk Operations
Write-Host "`n🔍 Testing Bulk Operations..." -ForegroundColor Magenta

# Create additional test logins for bulk operations
$bulkLogins = @()
for ($i = 1; $i -le 3; $i++) {
    $bulkLoginName = "bulkuser$i`_$(Get-Random)"
    $bulkLogins += $bulkLoginName
    
    $bulkLoginRequest = @{
        loginName = $bulkLoginName
        password = "BulkPass123!"
        loginType = "SQL"
        serverInstance = $ServerInstance
        defaultDatabase = "master"
    }
    
    Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/SqlLogin" -Body $bulkLoginRequest -Description "Create Bulk Test Login $i"
}

# Bulk disable operations
if ($bulkLogins.Count -gt 0) {
    $bulkDisableRequest = @{
        operation = "disable"
        serverInstance = $ServerInstance
        loginNames = $bulkLogins
    }
    
    Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/SqlLogin/bulk" -Body $bulkDisableRequest -Description "Bulk Disable Logins"
    
    # Bulk enable operations
    $bulkEnableRequest = @{
        operation = "enable"
        serverInstance = $ServerInstance
        loginNames = $bulkLogins
    }
    
    Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/SqlLogin/bulk" -Body $bulkEnableRequest -Description "Bulk Enable Logins"
}

# Cleanup: Delete all test objects
Write-Host "`n🧹 Cleaning up test objects..." -ForegroundColor Yellow

# Delete database user
if ($createdUser) {
    Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/DatabaseUser/$TestUserName/database/$DatabaseName/server/$ServerInstance" -Description "Delete Test Database User"
}

# Delete database role
if ($createdDBRole) {
    Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/DatabaseRole/$TestDatabaseRole/database/$DatabaseName/server/$ServerInstance" -Description "Delete Test Database Role"
}

# Delete server role
if ($createdServerRole) {
    Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/ServerRole/$TestServerRole/server/$ServerInstance" -Description "Delete Test Server Role"
}

# Delete bulk test logins
if ($bulkLogins.Count -gt 0) {
    $bulkDeleteRequest = @{
        operation = "delete"
        serverInstance = $ServerInstance
        loginNames = $bulkLogins
    }
    
    Invoke-ApiCall -Method "POST" -Uri "$BaseUrl/SqlLogin/bulk" -Body $bulkDeleteRequest -Description "Bulk Delete Test Logins"
}

# Delete test logins
if ($createdLogin) {
    Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/SqlLogin/$TestLoginName/server/$ServerInstance" -Description "Delete Test SQL Login"
}

if ($createdWindowsLogin) {
    Invoke-ApiCall -Method "DELETE" -Uri "$BaseUrl/SqlLogin/$TestWindowsLogin/server/$ServerInstance" -Description "Delete Test Windows Login"
}

Write-Host "`n✅ API Testing Complete!" -ForegroundColor Green
Write-Host "Check the output above for any failed tests." -ForegroundColor Yellow