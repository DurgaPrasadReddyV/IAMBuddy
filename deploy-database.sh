#!/bin/bash

# Bash script to deploy database migrations for IAMBuddy services
ENVIRONMENT="${1:-Development}"
REQUEST_INTAKE_ONLY="${2:-false}"
SQL_SERVER_MANAGEMENT_ONLY="${3:-false}"
WHAT_IF="${4:-false}"

echo "IAMBuddy Database Migration Deployment Script"
echo "Environment: $ENVIRONMENT"

# Set connection strings based on environment
case $ENVIRONMENT in
    "Development")
        REQUEST_INTAKE_CONNECTION_STRING="Server=(localdb)\\mssqllocaldb;Database=IAMBuddy_RequestIntake;Trusted_Connection=true;MultipleActiveResultSets=true"
        SQL_SERVER_MANAGEMENT_CONNECTION_STRING="Server=(localdb)\\mssqllocaldb;Database=IAMBuddy_SqlServerManagement;Trusted_Connection=true;MultipleActiveResultSets=true"
        ;;
    "Production")
        REQUEST_INTAKE_CONNECTION_STRING="${IAMBUDDY_REQUESTINTAKE_CONNECTION_STRING}"
        SQL_SERVER_MANAGEMENT_CONNECTION_STRING="${IAMBUDDY_SQLSERVERMANAGEMENT_CONNECTION_STRING}"
        ;;
    *)
        echo "Unknown environment: $ENVIRONMENT"
        exit 1
        ;;
esac

deploy_migration() {
    local SERVICE_PATH="$1"
    local SERVICE_NAME="$2"
    local CONNECTION_STRING="$3"
    
    echo "Deploying $SERVICE_NAME migrations..."
    
    if [ "$WHAT_IF" = "true" ]; then
        echo "WHAT-IF: Would execute migration for $SERVICE_NAME"
        cd "$SERVICE_PATH"
        dotnet ef database update --connection "$CONNECTION_STRING" --dry-run
    else
        cd "$SERVICE_PATH"
        if dotnet ef database update --connection "$CONNECTION_STRING"; then
            echo "$SERVICE_NAME migration completed successfully"
        else
            echo "Failed to deploy $SERVICE_NAME migration"
            exit 1
        fi
    fi
}

# Deploy RequestIntakeService migrations
if [ "$SQL_SERVER_MANAGEMENT_ONLY" != "true" ]; then
    deploy_migration "IAMBuddy.RequestIntakeService" "RequestIntakeService" "$REQUEST_INTAKE_CONNECTION_STRING"
fi

# Deploy SqlServerManagementService migrations
if [ "$REQUEST_INTAKE_ONLY" != "true" ]; then
    deploy_migration "IAMBuddy.SqlServerManagementService" "SqlServerManagementService" "$SQL_SERVER_MANAGEMENT_CONNECTION_STRING"
fi

echo "Database deployment completed!"