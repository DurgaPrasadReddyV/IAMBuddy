# PowerShell script to deploy database migrations for IAMBuddy services
param(
    [string]$Environment = "Development",
    [switch]$RequestIntakeOnly,
    [switch]$SqlServerManagementOnly,
    [switch]$WhatIf
)

Write-Host "IAMBuddy Database Migration Deployment Script" -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Yellow

# Set connection strings based on environment
$requestIntakeConnectionString = switch ($Environment) {
    "Development" { "Server=(localdb)\mssqllocaldb;Database=IAMBuddy_RequestIntake;Trusted_Connection=true;MultipleActiveResultSets=true" }
    "Production" { $env:IAMBUDDY_REQUESTINTAKE_CONNECTION_STRING }
    default { throw "Unknown environment: $Environment" }
}

$sqlServerManagementConnectionString = switch ($Environment) {
    "Development" { "Server=(localdb)\mssqllocaldb;Database=IAMBuddy_SqlServerManagement;Trusted_Connection=true;MultipleActiveResultSets=true" }
    "Production" { $env:IAMBUDDY_SQLSERVERMANAGEMENT_CONNECTION_STRING }
    default { throw "Unknown environment: $Environment" }
}

function Deploy-Migration {
    param(
        [string]$ServicePath,
        [string]$ServiceName,
        [string]$ConnectionString
    )
    
    Write-Host "Deploying $ServiceName migrations..." -ForegroundColor Blue
    
    if ($WhatIf) {
        Write-Host "WHAT-IF: Would execute migration for $ServiceName" -ForegroundColor Cyan
        Set-Location $ServicePath
        dotnet ef database update --connection $ConnectionString --dry-run
    } else {
        try {
            Set-Location $ServicePath
            dotnet ef database update --connection $ConnectionString
            Write-Host "$ServiceName migration completed successfully" -ForegroundColor Green
        }
        catch {
            Write-Error "Failed to deploy $ServiceName migration: $($_.Exception.Message)"
            throw
        }
    }
}

# Deploy RequestIntakeService migrations
if (-not $SqlServerManagementOnly) {
    Deploy-Migration -ServicePath "IAMBuddy.RequestIntakeService" -ServiceName "RequestIntakeService" -ConnectionString $requestIntakeConnectionString
}

# Deploy SqlServerManagementService migrations
if (-not $RequestIntakeOnly) {
    Deploy-Migration -ServicePath "IAMBuddy.SqlServerManagementService" -ServiceName "SqlServerManagementService" -ConnectionString $sqlServerManagementConnectionString
}

Write-Host "Database deployment completed!" -ForegroundColor Green