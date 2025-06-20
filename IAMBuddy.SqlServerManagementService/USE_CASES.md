# SQL Server Management Service Use Cases

This document provides comprehensive examples of common use cases and scenarios for the SQL Server Management Service. Each use case includes step-by-step instructions, code examples, and best practices.

## Table of Contents

- [New Employee Onboarding](#new-employee-onboarding)
- [Employee Role Change](#employee-role-change)
- [Employee Offboarding](#employee-offboarding)
- [Application Account Setup](#application-account-setup)
- [Database Migration](#database-migration)
- [Security Audit](#security-audit)
- [Bulk User Management](#bulk-user-management)
- [Disaster Recovery](#disaster-recovery)
- [Compliance Reporting](#compliance-reporting)
- [Development Environment Setup](#development-environment-setup)

## New Employee Onboarding

### Scenario
A new developer joins the team and needs access to multiple databases with appropriate permissions.

### Requirements
- Create SQL Server login
- Create database users in multiple databases
- Assign appropriate roles and permissions
- Follow security best practices

### Step-by-Step Implementation

#### 1. Create SQL Server Login

**PowerShell:**
```powershell
# Employee details
$employeeId = "EMP001"
$employeeName = "john.doe"
$temporaryPassword = "TempPass123!"
$serverInstance = "PROD-SQL01"

# Create SQL login
$loginRequest = @{
    loginName = $employeeName
    password = $temporaryPassword
    loginType = "SQL"
    serverInstance = $serverInstance
    defaultDatabase = "master"
}

$newLogin = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin" -Method POST -Body ($loginRequest | ConvertTo-Json) -ContentType "application/json"
Write-Host "Created login: $($newLogin.loginName)"
```

**cURL:**
```bash
# Create SQL login
curl -X POST "https://localhost:7020/api/SqlLogin" \
  -H "Content-Type: application/json" \
  -d '{
    "loginName": "john.doe",
    "password": "TempPass123!",
    "loginType": "SQL",
    "serverInstance": "PROD-SQL01",
    "defaultDatabase": "master"
  }'
```

#### 2. Create Database Users

**PowerShell:**
```powershell
# Databases the employee needs access to
$databases = @("ProjectDB", "ReportingDB", "TestDB")

foreach ($database in $databases) {
    $userRequest = @{
        userName = $employeeName
        loginName = $employeeName
        databaseName = $database
        serverInstance = $serverInstance
        defaultSchema = "dbo"
    }
    
    $newUser = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser" -Method POST -Body ($userRequest | ConvertTo-Json) -ContentType "application/json"
    Write-Host "Created user in $database: $($newUser.userName)"
}
```

#### 3. Assign Roles and Permissions

**PowerShell:**
```powershell
# Role assignments per database
$roleAssignments = @{
    "ProjectDB" = @("db_datareader", "db_datawriter")
    "ReportingDB" = @("db_datareader")
    "TestDB" = @("db_datareader", "db_datawriter", "db_ddladmin")
}

foreach ($database in $roleAssignments.Keys) {
    foreach ($role in $roleAssignments[$database]) {
        $roleRequest = @{
            memberName = $employeeName
            roleName = $role
            databaseName = $database
            serverInstance = $serverInstance
        }
        
        Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method POST -Body ($roleRequest | ConvertTo-Json) -ContentType "application/json"
        Write-Host "Assigned role $role to $employeeName in $database"
    }
}
```

#### 4. Force Password Change on First Login

**PowerShell:**
```powershell
# Note: This would typically be done through SQL Server directly
# as the API doesn't currently support MUST_CHANGE option
# This is a placeholder for the concept

$updateRequest = @{
    # Future enhancement: mustChangePassword = $true
    isEnabled = $true
}

Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin/$employeeName/server/$serverInstance" -Method PUT -Body ($updateRequest | ConvertTo-Json) -ContentType "application/json"
```

### Automation Script

```powershell
function New-EmployeeAccess {
    param(
        [Parameter(Mandatory)]
        [string]$EmployeeName,
        
        [Parameter(Mandatory)]
        [string]$ServerInstance,
        
        [Parameter(Mandatory)]
        [hashtable]$DatabaseRoles,
        
        [string]$DefaultPassword = "TempPass123!"
    )
    
    try {
        # Create login
        $loginRequest = @{
            loginName = $EmployeeName
            password = $DefaultPassword
            loginType = "SQL"
            serverInstance = $ServerInstance
            defaultDatabase = "master"
        }
        
        $login = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin" -Method POST -Body ($loginRequest | ConvertTo-Json) -ContentType "application/json"
        
        # Create users and assign roles
        foreach ($database in $DatabaseRoles.Keys) {
            # Create user
            $userRequest = @{
                userName = $EmployeeName
                loginName = $EmployeeName
                databaseName = $database
                serverInstance = $ServerInstance
                defaultSchema = "dbo"
            }
            
            $user = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser" -Method POST -Body ($userRequest | ConvertTo-Json) -ContentType "application/json"
            
            # Assign roles
            foreach ($role in $DatabaseRoles[$database]) {
                $roleRequest = @{
                    memberName = $EmployeeName
                    roleName = $role
                    databaseName = $database
                    serverInstance = $ServerInstance
                }
                
                Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method POST -Body ($roleRequest | ConvertTo-Json) -ContentType "application/json"
            }
        }
        
        Write-Host "Successfully created access for $EmployeeName"
    }
    catch {
        Write-Error "Failed to create access for $EmployeeName`: $($_.Exception.Message)"
    }
}

# Usage
$roleAssignments = @{
    "ProjectDB" = @("db_datareader", "db_datawriter")
    "ReportingDB" = @("db_datareader")
}

New-EmployeeAccess -EmployeeName "john.doe" -ServerInstance "PROD-SQL01" -DatabaseRoles $roleAssignments
```

## Employee Role Change

### Scenario
An employee is promoted from Developer to Senior Developer and needs additional permissions.

### Implementation

```powershell
function Update-EmployeePermissions {
    param(
        [Parameter(Mandatory)]
        [string]$EmployeeName,
        
        [Parameter(Mandatory)]
        [string]$ServerInstance,
        
        [Parameter(Mandatory)]
        [hashtable]$NewRoleAssignments,
        
        [hashtable]$RolesToRemove = @{}
    )
    
    # Remove old roles
    foreach ($database in $RolesToRemove.Keys) {
        foreach ($role in $RolesToRemove[$database]) {
            $removeRequest = @{
                memberName = $EmployeeName
                roleName = $role
                databaseName = $database
                serverInstance = $ServerInstance
            }
            
            try {
                Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method DELETE -Body ($removeRequest | ConvertTo-Json) -ContentType "application/json"
                Write-Host "Removed role $role from $EmployeeName in $database"
            }
            catch {
                Write-Warning "Failed to remove role $role from $EmployeeName in $database`: $($_.Exception.Message)"
            }
        }
    }
    
    # Add new roles
    foreach ($database in $NewRoleAssignments.Keys) {
        foreach ($role in $NewRoleAssignments[$database]) {
            $addRequest = @{
                memberName = $EmployeeName
                roleName = $role
                databaseName = $database
                serverInstance = $ServerInstance
            }
            
            try {
                Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method POST -Body ($addRequest | ConvertTo-Json) -ContentType "application/json"
                Write-Host "Added role $role to $EmployeeName in $database"
            }
            catch {
                Write-Warning "Failed to add role $role to $EmployeeName in $database`: $($_.Exception.Message)"
            }
        }
    }
}

# Example: Promote developer to senior developer
$newRoles = @{
    "ProjectDB" = @("db_ddladmin")  # Can now modify schema
    "ReportingDB" = @("db_datawriter")  # Can now modify reporting data
}

$rolesToRemove = @{
    # No roles to remove in this case
}

Update-EmployeePermissions -EmployeeName "john.doe" -ServerInstance "PROD-SQL01" -NewRoleAssignments $newRoles -RolesToRemove $rolesToRemove
```

## Employee Offboarding

### Scenario
An employee is leaving the company and all access must be revoked immediately.

### Implementation

```powershell
function Remove-EmployeeAccess {
    param(
        [Parameter(Mandatory)]
        [string]$EmployeeName,
        
        [Parameter(Mandatory)]
        [string]$ServerInstance,
        
        [switch]$DisableOnly,  # Disable instead of delete for audit purposes
        
        [string[]]$DatabaseNames = @()
    )
    
    try {
        # Get all databases where user exists if not specified
        if ($DatabaseNames.Count -eq 0) {
            $usersByLogin = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/login/$EmployeeName/server/$ServerInstance" -Method GET
            $DatabaseNames = $usersByLogin | ForEach-Object { $_.databaseName } | Sort-Object -Unique
        }
        
        if ($DisableOnly) {
            # Disable login
            $disableRequest = @{ isEnabled = $false }
            Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin/$EmployeeName/server/$ServerInstance" -Method PUT -Body ($disableRequest | ConvertTo-Json) -ContentType "application/json"
            Write-Host "Disabled login for $EmployeeName"
        }
        else {
            # Delete database users first
            foreach ($database in $DatabaseNames) {
                try {
                    Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/$EmployeeName/database/$database/server/$ServerInstance" -Method DELETE
                    Write-Host "Deleted user $EmployeeName from database $database"
                }
                catch {
                    Write-Warning "Failed to delete user $EmployeeName from database $database`: $($_.Exception.Message)"
                }
            }
            
            # Delete login
            Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin/$EmployeeName/server/$ServerInstance" -Method DELETE
            Write-Host "Deleted login $EmployeeName"
        }
        
        Write-Host "Successfully removed access for $EmployeeName"
    }
    catch {
        Write-Error "Failed to remove access for $EmployeeName`: $($_.Exception.Message)"
    }
}

# Usage
Remove-EmployeeAccess -EmployeeName "john.doe" -ServerInstance "PROD-SQL01" -DisableOnly
```

## Application Account Setup

### Scenario
Setting up a service account for an application with specific database permissions.

### Implementation

```powershell
function New-ApplicationAccount {
    param(
        [Parameter(Mandatory)]
        [string]$ApplicationName,
        
        [Parameter(Mandatory)]
        [string]$ServerInstance,
        
        [Parameter(Mandatory)]
        [string]$DatabaseName,
        
        [string[]]$RequiredPermissions = @("SELECT", "INSERT", "UPDATE", "DELETE"),
        
        [string]$AccountPassword
    )
    
    $accountName = "svc_$($ApplicationName.ToLower())"
    
    if (-not $AccountPassword) {
        # Generate secure password
        $AccountPassword = -join ((33..126) | Get-Random -Count 16 | ForEach-Object { [char]$_ })
    }
    
    try {
        # Create login
        $loginRequest = @{
            loginName = $accountName
            password = $AccountPassword
            loginType = "SQL"
            serverInstance = $ServerInstance
            defaultDatabase = $DatabaseName
        }
        
        $login = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin" -Method POST -Body ($loginRequest | ConvertTo-Json) -ContentType "application/json"
        
        # Create database user
        $userRequest = @{
            userName = $accountName
            loginName = $accountName
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
            defaultSchema = "dbo"
        }
        
        $user = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser" -Method POST -Body ($userRequest | ConvertTo-Json) -ContentType "application/json"
        
        # Create custom role for the application
        $roleName = "role_$($ApplicationName.ToLower())"
        $roleRequest = @{
            roleName = $roleName
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
        }
        
        $role = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseRole" -Method POST -Body ($roleRequest | ConvertTo-Json) -ContentType "application/json"
        
        # Assign user to role
        $memberRequest = @{
            roleName = $roleName
            memberName = $accountName
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
        }
        
        Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseRole/members" -Method POST -Body ($memberRequest | ConvertTo-Json) -ContentType "application/json"
        
        # Grant permissions to role (this would typically be done through SQL)
        foreach ($permission in $RequiredPermissions) {
            $permissionRequest = @{
                roleName = $roleName
                permission = $permission
                databaseName = $DatabaseName
                serverInstance = $ServerInstance
                objectName = "ALL_TABLES"  # Application-specific
            }
            
            try {
                Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseRole/permissions" -Method POST -Body ($permissionRequest | ConvertTo-Json) -ContentType "application/json"
            }
            catch {
                Write-Warning "Permission $permission might need to be granted manually"
            }
        }
        
        # Return account details
        @{
            AccountName = $accountName
            Password = $AccountPassword
            ConnectionString = "Server=$ServerInstance;Database=$DatabaseName;User Id=$accountName;Password=$AccountPassword;Encrypt=true;TrustServerCertificate=false;"
        }
    }
    catch {
        Write-Error "Failed to create application account: $($_.Exception.Message)"
    }
}

# Usage
$appAccount = New-ApplicationAccount -ApplicationName "OrderService" -ServerInstance "PROD-SQL01" -DatabaseName "OrdersDB"
Write-Host "Application account created: $($appAccount.AccountName)"
Write-Host "Connection string: $($appAccount.ConnectionString)"
```

## Database Migration

### Scenario
Migrating user accounts from one SQL Server instance to another.

### Implementation

```powershell
function Copy-DatabaseAccess {
    param(
        [Parameter(Mandatory)]
        [string]$SourceServer,
        
        [Parameter(Mandatory)]
        [string]$TargetServer,
        
        [Parameter(Mandatory)]
        [string]$DatabaseName,
        
        [string[]]$UsersToMigrate = @(),  # Empty means all users
        
        [switch]$CreateLoginsIfMissing
    )
    
    try {
        # Get source database users
        $sourceUsers = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/database/$DatabaseName/server/$SourceServer" -Method GET
        
        if ($UsersToMigrate.Count -gt 0) {
            $sourceUsers = $sourceUsers | Where-Object { $_.userName -in $UsersToMigrate }
        }
        
        foreach ($user in $sourceUsers) {
            Write-Host "Migrating user: $($user.userName)"
            
            # Check if login exists on target server
            $loginExists = $false
            try {
                $login = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin/$($user.loginName)/server/$TargetServer" -Method GET
                $loginExists = $true
            }
            catch {
                # Login doesn't exist
            }
            
            # Create login if missing and requested
            if (-not $loginExists -and $CreateLoginsIfMissing -and $user.loginName) {
                Write-Host "Creating missing login: $($user.loginName)"
                $tempPassword = -join ((33..126) | Get-Random -Count 16 | ForEach-Object { [char]$_ })
                
                $loginRequest = @{
                    loginName = $user.loginName
                    password = $tempPassword
                    loginType = "SQL"
                    serverInstance = $TargetServer
                    defaultDatabase = $DatabaseName
                }
                
                try {
                    Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin" -Method POST -Body ($loginRequest | ConvertTo-Json) -ContentType "application/json"
                    Write-Host "Created login with temporary password (must be changed)"
                }
                catch {
                    Write-Warning "Failed to create login $($user.loginName): $($_.Exception.Message)"
                    continue
                }
            }
            
            # Create database user
            $userRequest = @{
                userName = $user.userName
                loginName = $user.loginName
                databaseName = $DatabaseName
                serverInstance = $TargetServer
                defaultSchema = $user.defaultSchema
            }
            
            try {
                $newUser = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser" -Method POST -Body ($userRequest | ConvertTo-Json) -ContentType "application/json"
                
                # Get source user roles
                $sourceRoles = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/$($user.userName)/database/$DatabaseName/server/$SourceServer/roles" -Method GET
                
                # Assign roles to new user
                foreach ($role in $sourceRoles) {
                    $roleRequest = @{
                        memberName = $user.userName
                        roleName = $role
                        databaseName = $DatabaseName
                        serverInstance = $TargetServer
                    }
                    
                    try {
                        Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method POST -Body ($roleRequest | ConvertTo-Json) -ContentType "application/json"
                    }
                    catch {
                        Write-Warning "Failed to assign role $role to $($user.userName): $($_.Exception.Message)"
                    }
                }
                
                Write-Host "Successfully migrated user: $($user.userName)"
            }
            catch {
                Write-Warning "Failed to create user $($user.userName): $($_.Exception.Message)"
            }
        }
    }
    catch {
        Write-Error "Migration failed: $($_.Exception.Message)"
    }
}

# Usage
Copy-DatabaseAccess -SourceServer "OLD-SQL01" -TargetServer "NEW-SQL01" -DatabaseName "ProjectDB" -CreateLoginsIfMissing
```

## Security Audit

### Scenario
Generating a comprehensive security audit report for compliance purposes.

### Implementation

```powershell
function Get-SecurityAuditReport {
    param(
        [Parameter(Mandatory)]
        [string]$ServerInstance,
        
        [string[]]$DatabaseNames = @(),
        
        [string]$OutputPath = ".\SecurityAudit_$(Get-Date -Format 'yyyyMMdd_HHmmss').html"
    )
    
    $auditReport = @{
        GeneratedDate = Get-Date
        ServerInstance = $ServerInstance
        Logins = @()
        ServerRoles = @()
        DatabaseUsers = @{}
        DatabaseRoles = @{}
        SecurityFindings = @()
    }
    
    try {
        # Get all logins
        $auditReport.Logins = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin/server/$ServerInstance" -Method GET
        
        # Get server roles
        $auditReport.ServerRoles = Invoke-RestMethod -Uri "https://localhost:7020/api/ServerRole/server/$ServerInstance" -Method GET
        
        # If no specific databases, get all databases with users
        if ($DatabaseNames.Count -eq 0) {
            # This would typically come from a database catalog query
            $DatabaseNames = @("master", "model", "msdb", "tempdb")  # System databases as example
        }
        
        foreach ($database in $DatabaseNames) {
            # Get database users
            try {
                $auditReport.DatabaseUsers[$database] = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/database/$database/server/$ServerInstance" -Method GET
            }
            catch {
                $auditReport.SecurityFindings += "Warning: Could not access database $database"
            }
            
            # Get database roles
            try {
                $auditReport.DatabaseRoles[$database] = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseRole/database/$database/server/$ServerInstance" -Method GET
            }
            catch {
                $auditReport.SecurityFindings += "Warning: Could not access database roles for $database"
            }
        }
        
        # Security analysis
        $disabledLogins = $auditReport.Logins | Where-Object { -not $_.isEnabled }
        if ($disabledLogins.Count -gt 0) {
            $auditReport.SecurityFindings += "Found $($disabledLogins.Count) disabled login(s): $($disabledLogins.loginName -join ', ')"
        }
        
        $lockedLogins = $auditReport.Logins | Where-Object { $_.isLocked }
        if ($lockedLogins.Count -gt 0) {
            $auditReport.SecurityFindings += "Found $($lockedLogins.Count) locked login(s): $($lockedLogins.loginName -join ', ')"
        }
        
        $sysadminMembers = @()
        foreach ($role in $auditReport.ServerRoles | Where-Object { $_.roleName -eq "sysadmin" }) {
            try {
                $members = Invoke-RestMethod -Uri "https://localhost:7020/api/ServerRole/sysadmin/server/$ServerInstance/members" -Method GET
                $sysadminMembers += $members
            }
            catch {
                $auditReport.SecurityFindings += "Warning: Could not retrieve sysadmin role members"
            }
        }
        
        if ($sysadminMembers.Count -gt 5) {
            $auditReport.SecurityFindings += "Warning: High number of sysadmin members ($($sysadminMembers.Count)): $($sysadminMembers -join ', ')"
        }
        
        # Generate HTML report
        $html = @"
<!DOCTYPE html>
<html>
<head>
    <title>SQL Server Security Audit Report</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        h1, h2 { color: #2c3e50; }
        table { border-collapse: collapse; width: 100%; margin: 10px 0; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        .warning { color: #e74c3c; font-weight: bold; }
        .info { background-color: #ecf0f1; padding: 10px; margin: 10px 0; }
    </style>
</head>
<body>
    <h1>SQL Server Security Audit Report</h1>
    <div class="info">
        <strong>Generated:</strong> $($auditReport.GeneratedDate)<br>
        <strong>Server Instance:</strong> $($auditReport.ServerInstance)
    </div>
    
    <h2>Security Findings</h2>
    <ul>
"@
        
        foreach ($finding in $auditReport.SecurityFindings) {
            $html += "<li class='warning'>$finding</li>`n"
        }
        
        $html += @"
    </ul>
    
    <h2>SQL Server Logins ($($auditReport.Logins.Count))</h2>
    <table>
        <tr><th>Login Name</th><th>Type</th><th>Enabled</th><th>Locked</th><th>Last Login</th></tr>
"@
        
        foreach ($login in $auditReport.Logins) {
            $enabledStatus = if ($login.isEnabled) { "Yes" } else { "<span class='warning'>No</span>" }
            $lockedStatus = if ($login.isLocked) { "<span class='warning'>Yes</span>" } else { "No" }
            
            $html += "<tr><td>$($login.loginName)</td><td>$($login.loginType)</td><td>$enabledStatus</td><td>$lockedStatus</td><td>$($login.lastLoginDate)</td></tr>`n"
        }
        
        $html += "</table>"
        
        # Add database sections
        foreach ($database in $DatabaseNames) {
            if ($auditReport.DatabaseUsers[$database]) {
                $html += @"
    <h2>Database Users - $database ($($auditReport.DatabaseUsers[$database].Count))</h2>
    <table>
        <tr><th>User Name</th><th>Login Name</th><th>Type</th><th>Default Schema</th><th>Enabled</th></tr>
"@
                
                foreach ($user in $auditReport.DatabaseUsers[$database]) {
                    $enabledStatus = if ($user.isEnabled) { "Yes" } else { "<span class='warning'>No</span>" }
                    $html += "<tr><td>$($user.userName)</td><td>$($user.loginName)</td><td>$($user.userType)</td><td>$($user.defaultSchema)</td><td>$enabledStatus</td></tr>`n"
                }
                
                $html += "</table>"
            }
        }
        
        $html += @"
</body>
</html>
"@
        
        $html | Out-File -FilePath $OutputPath -Encoding UTF8
        Write-Host "Security audit report generated: $OutputPath"
        
        return $auditReport
    }
    catch {
        Write-Error "Failed to generate security audit report: $($_.Exception.Message)"
    }
}

# Usage
$auditReport = Get-SecurityAuditReport -ServerInstance "PROD-SQL01" -DatabaseNames @("ProjectDB", "ReportingDB")
```

## Bulk User Management

### Scenario
Processing a CSV file to create multiple users with different role assignments.

### Implementation

```powershell
function Import-UsersFromCsv {
    param(
        [Parameter(Mandatory)]
        [string]$CsvPath,
        
        [Parameter(Mandatory)]
        [string]$ServerInstance,
        
        [switch]$WhatIf
    )
    
    try {
        $users = Import-Csv -Path $CsvPath
        $results = @()
        
        foreach ($user in $users) {
            $result = @{
                UserName = $user.UserName
                Status = "Processing"
                Details = @()
                Errors = @()
            }
            
            try {
                if (-not $WhatIf) {
                    # Create login
                    $loginRequest = @{
                        loginName = $user.LoginName
                        password = $user.TempPassword
                        loginType = $user.LoginType
                        serverInstance = $ServerInstance
                        defaultDatabase = $user.DefaultDatabase
                    }
                    
                    $login = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin" -Method POST -Body ($loginRequest | ConvertTo-Json) -ContentType "application/json"
                    $result.Details += "Created login: $($user.LoginName)"
                }
                else {
                    $result.Details += "Would create login: $($user.LoginName)"
                }
                
                # Process each database access
                $databases = $user.DatabaseAccess -split ';'
                foreach ($dbAccess in $databases) {
                    $dbInfo = $dbAccess -split ':'
                    $dbName = $dbInfo[0]
                    $roles = $dbInfo[1] -split ','
                    
                    if (-not $WhatIf) {
                        # Create database user
                        $userRequest = @{
                            userName = $user.UserName
                            loginName = $user.LoginName
                            databaseName = $dbName
                            serverInstance = $ServerInstance
                            defaultSchema = $user.DefaultSchema
                        }
                        
                        $dbUser = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser" -Method POST -Body ($userRequest | ConvertTo-Json) -ContentType "application/json"
                        $result.Details += "Created user in $dbName"
                        
                        # Assign roles
                        foreach ($role in $roles) {
                            $roleRequest = @{
                                memberName = $user.UserName
                                roleName = $role.Trim()
                                databaseName = $dbName
                                serverInstance = $ServerInstance
                            }
                            
                            Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method POST -Body ($roleRequest | ConvertTo-Json) -ContentType "application/json"
                            $result.Details += "Assigned role $($role.Trim()) in $dbName"
                        }
                    }
                    else {
                        $result.Details += "Would create user in $dbName with roles: $($roles -join ', ')"
                    }
                }
                
                $result.Status = "Success"
            }
            catch {
                $result.Status = "Failed"
                $result.Errors += $_.Exception.Message
            }
            
            $results += $result
        }
        
        # Generate summary report
        $successCount = ($results | Where-Object { $_.Status -eq "Success" }).Count
        $failCount = ($results | Where-Object { $_.Status -eq "Failed" }).Count
        
        Write-Host "`nBulk User Import Summary:" -ForegroundColor Green
        Write-Host "Total Users: $($results.Count)"
        Write-Host "Successful: $successCount" -ForegroundColor Green
        Write-Host "Failed: $failCount" -ForegroundColor Red
        
        if ($failCount -gt 0) {
            Write-Host "`nFailed Users:" -ForegroundColor Red
            $results | Where-Object { $_.Status -eq "Failed" } | ForEach-Object {
                Write-Host "- $($_.UserName): $($_.Errors -join ', ')" -ForegroundColor Red
            }
        }
        
        return $results
    }
    catch {
        Write-Error "Failed to import users from CSV: $($_.Exception.Message)"
    }
}

# CSV Format Example:
# UserName,LoginName,TempPassword,LoginType,DefaultDatabase,DefaultSchema,DatabaseAccess
# john.doe,john.doe,TempPass123!,SQL,master,dbo,"ProjectDB:db_datareader,db_datawriter;ReportingDB:db_datareader"
# jane.smith,jane.smith,TempPass456!,SQL,master,dbo,"ProjectDB:db_datareader;TestDB:db_datareader,db_datawriter"

# Usage
$results = Import-UsersFromCsv -CsvPath ".\users.csv" -ServerInstance "PROD-SQL01" -WhatIf
```

## Development Environment Setup

### Scenario
Setting up a complete development environment with test data and appropriate permissions.

### Implementation

```powershell
function Initialize-DevelopmentEnvironment {
    param(
        [Parameter(Mandatory)]
        [string]$ServerInstance,
        
        [Parameter(Mandatory)]
        [string]$DatabaseName,
        
        [string[]]$Developers = @(),
        
        [switch]$CreateTestData
    )
    
    $devRoleName = "dev_role"
    $testRoleName = "test_role"
    
    try {
        # Create custom development roles
        $devRoleRequest = @{
            roleName = $devRoleName
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
        }
        
        $testRoleRequest = @{
            roleName = $testRoleName
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
        }
        
        $devRole = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseRole" -Method POST -Body ($devRoleRequest | ConvertTo-Json) -ContentType "application/json"
        $testRole = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseRole" -Method POST -Body ($testRoleRequest | ConvertTo-Json) -ContentType "application/json"
        
        Write-Host "Created development roles"
        
        # Grant permissions to development role
        $devPermissions = @("SELECT", "INSERT", "UPDATE", "DELETE")
        foreach ($permission in $devPermissions) {
            $permRequest = @{
                roleName = $devRoleName
                permission = $permission
                databaseName = $DatabaseName
                serverInstance = $ServerInstance
                objectName = "ALL_TABLES"
            }
            
            try {
                Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseRole/permissions" -Method POST -Body ($permRequest | ConvertTo-Json) -ContentType "application/json"
            }
            catch {
                Write-Warning "Permission $permission may need manual configuration"
            }
        }
        
        # Setup developer accounts
        foreach ($developer in $Developers) {
            $devLoginName = "dev_$developer"
            $tempPassword = "DevPass123!"
            
            # Create login
            $loginRequest = @{
                loginName = $devLoginName
                password = $tempPassword
                loginType = "SQL"
                serverInstance = $ServerInstance
                defaultDatabase = $DatabaseName
            }
            
            try {
                $login = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin" -Method POST -Body ($loginRequest | ConvertTo-Json) -ContentType "application/json"
                
                # Create database user
                $userRequest = @{
                    userName = $devLoginName
                    loginName = $devLoginName
                    databaseName = $DatabaseName
                    serverInstance = $ServerInstance
                    defaultSchema = "dbo"
                }
                
                $user = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser" -Method POST -Body ($userRequest | ConvertTo-Json) -ContentType "application/json"
                
                # Assign to development role
                $roleAssignRequest = @{
                    memberName = $devLoginName
                    roleName = $devRoleName
                    databaseName = $DatabaseName
                    serverInstance = $ServerInstance
                }
                
                Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method POST -Body ($roleAssignRequest | ConvertTo-Json) -ContentType "application/json"
                
                Write-Host "Setup developer account: $devLoginName"
            }
            catch {
                Write-Warning "Failed to setup developer account for $developer`: $($_.Exception.Message)"
            }
        }
        
        # Create test data account
        $testAccountRequest = @{
            loginName = "test_data_loader"
            password = "TestLoad123!"
            loginType = "SQL"
            serverInstance = $ServerInstance
            defaultDatabase = $DatabaseName
        }
        
        $testLogin = Invoke-RestMethod -Uri "https://localhost:7020/api/SqlLogin" -Method POST -Body ($testAccountRequest | ConvertTo-Json) -ContentType "application/json"
        
        $testUserRequest = @{
            userName = "test_data_loader"
            loginName = "test_data_loader"
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
            defaultSchema = "dbo"
        }
        
        $testUser = Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser" -Method POST -Body ($testUserRequest | ConvertTo-Json) -ContentType "application/json"
        
        # Assign bulk insert permissions to test account
        $testRoleAssignRequest = @{
            memberName = "test_data_loader"
            roleName = "db_datawriter"
            databaseName = $DatabaseName
            serverInstance = $ServerInstance
        }
        
        Invoke-RestMethod -Uri "https://localhost:7020/api/DatabaseUser/roles" -Method POST -Body ($testRoleAssignRequest | ConvertTo-Json) -ContentType "application/json"
        
        Write-Host "Development environment initialized successfully"
        
        # Return setup summary
        @{
            DatabaseName = $DatabaseName
            ServerInstance = $ServerInstance
            DeveloperAccounts = $Developers | ForEach-Object { "dev_$_" }
            TestAccount = "test_data_loader"
            Roles = @($devRoleName, $testRoleName)
        }
    }
    catch {
        Write-Error "Failed to initialize development environment: $($_.Exception.Message)"
    }
}

# Usage
$devSetup = Initialize-DevelopmentEnvironment -ServerInstance "DEV-SQL01" -DatabaseName "ProjectDB_Dev" -Developers @("alice", "bob", "charlie")
```

## Best Practices Summary

### Security Best Practices
1. **Principle of Least Privilege**: Grant minimum necessary permissions
2. **Regular Access Reviews**: Audit user access quarterly
3. **Strong Password Policies**: Enforce complex passwords and regular changes
4. **Account Monitoring**: Monitor for unusual access patterns
5. **Separation of Duties**: Separate administrative and application accounts

### Operational Best Practices
1. **Automation**: Use scripts for consistent user provisioning
2. **Documentation**: Maintain records of all access grants and changes
3. **Testing**: Test all scripts in development environments first
4. **Error Handling**: Implement comprehensive error handling and logging
5. **Rollback Plans**: Have procedures to quickly revoke access if needed

### Performance Best Practices
1. **Bulk Operations**: Use bulk APIs for multiple operations
2. **Connection Pooling**: Reuse connections when possible
3. **Async Operations**: Use asynchronous calls for better throughput
4. **Caching**: Cache frequently accessed data
5. **Monitoring**: Monitor API performance and database impact

These use cases provide a comprehensive foundation for implementing common SQL Server identity management scenarios using the SQL Server Management Service API.