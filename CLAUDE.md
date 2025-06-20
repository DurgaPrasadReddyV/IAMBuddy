# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

- **Build entire solution**: `dotnet build`
- **Run the application**: `dotnet run --project IAMBuddy.AppHost`
- **Build specific project**: `dotnet build IAMBuddy.{ServiceName}`
- **Run specific service**: `dotnet run --project IAMBuddy.{ServiceName}`

## Architecture Overview

IAMBuddy is a microservices-based Identity and Access Management (IAM) system built on .NET 9 using .NET Aspire for orchestration and Temporal for workflow management.

### Service Architecture

The system consists of 6 main services orchestrated by an AppHost:

1. **RequestIntakeService** - Entry point for MSSQL account requests with Entity Framework DbContext
2. **ApprovalService** - Handles approval workflows for account requests
3. **ProvisioningService** - Executes actual account provisioning
4. **NotificationService** - Manages email notifications via EmailService
5. **TemporalWorker** - Processes Temporal workflows and activities
6. **AppHost** - .NET Aspire orchestration host that manages all services

### Key Components

- **Shared Library** (`IAMBuddy.Shared`) - Contains common models, workflows, and activities
- **ServiceDefaults** - Common configuration and extensions
- **Temporal Integration** - Workflow orchestration using Temporal.io with custom activities
- **Database** - Entity Framework with `AppDbContext` for request persistence

### Workflow Flow

1. Request submitted to RequestIntakeService
2. Validation performed via RequestValidationService
3. Request stored in database with EF Core
4. Temporal workflow started via TemporalClientService
5. Workflow coordinates approval, provisioning, and notification services

### Important Files

- `IAMBuddy.Shared/Workflows/MSSQLAccountProvisioningWorkflow.cs` - Main workflow definition
- `IAMBuddy.Shared/Models/MSSQLAccountRequest.cs` - Core request model
- `IAMBuddy.AppHost/Program.cs` - Service orchestration configuration
- `IAMBuddy.RequestIntakeService/AppDbContext.cs` - EF Core database context