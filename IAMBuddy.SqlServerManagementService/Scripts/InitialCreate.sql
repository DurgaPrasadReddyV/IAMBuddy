IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [SqlServerLogins] (
    [Id] int NOT NULL IDENTITY,
    [LoginName] nvarchar(128) NOT NULL,
    [LoginType] nvarchar(50) NOT NULL,
    [Sid] nvarchar(256) NULL,
    [IsEnabled] bit NOT NULL,
    [IsLocked] bit NOT NULL,
    [PasswordExpiryDate] datetime2 NULL,
    [LastLoginDate] datetime2 NULL,
    [ServerInstance] nvarchar(128) NOT NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [ModifiedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] nvarchar(100) NOT NULL,
    [ModifiedBy] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_SqlServerLogins] PRIMARY KEY ([Id])
);

CREATE TABLE [SqlServerRoles] (
    [Id] int NOT NULL IDENTITY,
    [RoleName] nvarchar(128) NOT NULL,
    [RoleType] nvarchar(50) NOT NULL,
    [DatabaseName] nvarchar(128) NULL,
    [ServerInstance] nvarchar(128) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsBuiltIn] bit NOT NULL,
    [IsEnabled] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [ModifiedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] nvarchar(100) NOT NULL,
    [ModifiedBy] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_SqlServerRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [SqlServerUsers] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(128) NOT NULL,
    [DatabaseName] nvarchar(128) NOT NULL,
    [ServerInstance] nvarchar(128) NOT NULL,
    [Sid] nvarchar(256) NULL,
    [IsEnabled] bit NOT NULL,
    [UserType] nvarchar(50) NOT NULL,
    [DefaultSchema] nvarchar(128) NULL,
    [LoginId] int NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [ModifiedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] nvarchar(100) NOT NULL,
    [ModifiedBy] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_SqlServerUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SqlServerUsers_SqlServerLogins_LoginId] FOREIGN KEY ([LoginId]) REFERENCES [SqlServerLogins] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [SqlServerRoleAssignments] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [LoginId] int NULL,
    [UserId] int NULL,
    [ServerInstance] nvarchar(128) NOT NULL,
    [DatabaseName] nvarchar(128) NULL,
    [IsActive] bit NOT NULL,
    [EffectiveDate] datetime2 NULL,
    [ExpiryDate] datetime2 NULL,
    [AssignmentReason] nvarchar(500) NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [ModifiedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] nvarchar(100) NOT NULL,
    [ModifiedBy] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_SqlServerRoleAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_RoleAssignment_LoginOrUser] CHECK ((LoginId IS NOT NULL AND UserId IS NULL) OR (LoginId IS NULL AND UserId IS NOT NULL)),
    CONSTRAINT [FK_SqlServerRoleAssignments_SqlServerLogins_LoginId] FOREIGN KEY ([LoginId]) REFERENCES [SqlServerLogins] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SqlServerRoleAssignments_SqlServerRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [SqlServerRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SqlServerRoleAssignments_SqlServerUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [SqlServerUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [SqlServerOperations] (
    [Id] int NOT NULL IDENTITY,
    [OperationType] nvarchar(50) NOT NULL,
    [ResourceType] nvarchar(50) NOT NULL,
    [ServerInstance] nvarchar(128) NOT NULL,
    [DatabaseName] nvarchar(128) NULL,
    [ResourceName] nvarchar(255) NULL,
    [OperationDetails] nvarchar(max) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ErrorMessage] nvarchar(1000) NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NULL,
    [DurationMs] int NULL,
    [LoginId] int NULL,
    [UserId] int NULL,
    [RoleId] int NULL,
    [RoleAssignmentId] int NULL,
    [CreatedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [CreatedBy] nvarchar(100) NOT NULL,
    [RequestId] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_SqlServerOperations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SqlServerOperations_SqlServerLogins_LoginId] FOREIGN KEY ([LoginId]) REFERENCES [SqlServerLogins] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_SqlServerOperations_SqlServerRoleAssignments_RoleAssignmentId] FOREIGN KEY ([RoleAssignmentId]) REFERENCES [SqlServerRoleAssignments] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_SqlServerOperations_SqlServerRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [SqlServerRoles] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_SqlServerOperations_SqlServerUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [SqlServerUsers] ([Id]) ON DELETE SET NULL
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'DatabaseName', N'Description', N'IsBuiltIn', N'IsEnabled', N'ModifiedBy', N'ModifiedDate', N'RoleName', N'RoleType', N'ServerInstance') AND [object_id] = OBJECT_ID(N'[SqlServerRoles]'))
    SET IDENTITY_INSERT [SqlServerRoles] ON;
INSERT INTO [SqlServerRoles] ([Id], [CreatedBy], [CreatedDate], [DatabaseName], [Description], [IsBuiltIn], [IsEnabled], [ModifiedBy], [ModifiedDate], [RoleName], [RoleType], [ServerInstance])
VALUES (1, N'System', '2025-06-20T11:04:31.8197240Z', NULL, N'Members can perform any activity in the server', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8197240Z', N'sysadmin', N'Server', N'Default'),
(2, N'System', '2025-06-20T11:04:31.8198400Z', NULL, N'Members can change server-wide configuration options and shut down the server', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198400Z', N'serveradmin', N'Server', N'Default'),
(3, N'System', '2025-06-20T11:04:31.8198400Z', NULL, N'Members can manage logins and their properties', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198400Z', N'securityadmin', N'Server', N'Default'),
(4, N'System', '2025-06-20T11:04:31.8198400Z', NULL, N'Members can end processes that are running in an instance of SQL Server', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198400Z', N'processadmin', N'Server', N'Default'),
(5, N'System', '2025-06-20T11:04:31.8198400Z', NULL, N'Members can add and remove linked servers', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198400Z', N'setupadmin', N'Server', N'Default'),
(6, N'System', '2025-06-20T11:04:31.8198400Z', NULL, N'Members can run the BULK INSERT statement', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198400Z', N'bulkadmin', N'Server', N'Default'),
(7, N'System', '2025-06-20T11:04:31.8198410Z', NULL, N'Members can manage disk files', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198410Z', N'diskadmin', N'Server', N'Default'),
(8, N'System', '2025-06-20T11:04:31.8198410Z', NULL, N'Members can create, alter, drop, and restore any database', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198410Z', N'dbcreator', N'Server', N'Default'),
(9, N'System', '2025-06-20T11:04:31.8198410Z', NULL, N'Every SQL Server login belongs to the public server role', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198410Z', N'public', N'Server', N'Default'),
(10, N'System', '2025-06-20T11:04:31.8198410Z', N'Default', N'Members can perform all configuration and maintenance activities on the database', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198410Z', N'db_owner', N'Database', N'Default'),
(11, N'System', '2025-06-20T11:04:31.8198540Z', N'Default', N'Members can add or remove access to the database for Windows logins, Windows groups, and SQL Server logins', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198540Z', N'db_accessadmin', N'Database', N'Default'),
(12, N'System', '2025-06-20T11:04:31.8198540Z', N'Default', N'Members can modify role membership and manage permissions', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198540Z', N'db_securityadmin', N'Database', N'Default'),
(13, N'System', '2025-06-20T11:04:31.8198540Z', N'Default', N'Members can run any Data Definition Language (DDL) command in a database', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198540Z', N'db_ddladmin', N'Database', N'Default'),
(14, N'System', '2025-06-20T11:04:31.8198540Z', N'Default', N'Members can back up the database', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198540Z', N'db_backupoperator', N'Database', N'Default'),
(15, N'System', '2025-06-20T11:04:31.8198550Z', N'Default', N'Members can read all data from all user tables', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198550Z', N'db_datareader', N'Database', N'Default'),
(16, N'System', '2025-06-20T11:04:31.8198550Z', N'Default', N'Members can add, change, or delete data from all user tables', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198550Z', N'db_datawriter', N'Database', N'Default'),
(17, N'System', '2025-06-20T11:04:31.8198550Z', N'Default', N'Members cannot read any data in the user tables within a database', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198550Z', N'db_denydatareader', N'Database', N'Default'),
(18, N'System', '2025-06-20T11:04:31.8198550Z', N'Default', N'Members cannot add, modify, or delete any data in the user tables within a database', CAST(1 AS bit), CAST(1 AS bit), N'System', '2025-06-20T11:04:31.8198550Z', N'db_denydatawriter', N'Database', N'Default');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedBy', N'CreatedDate', N'DatabaseName', N'Description', N'IsBuiltIn', N'IsEnabled', N'ModifiedBy', N'ModifiedDate', N'RoleName', N'RoleType', N'ServerInstance') AND [object_id] = OBJECT_ID(N'[SqlServerRoles]'))
    SET IDENTITY_INSERT [SqlServerRoles] OFF;

CREATE UNIQUE INDEX [IX_SqlServerLogins_LoginName_ServerInstance] ON [SqlServerLogins] ([LoginName], [ServerInstance]);

CREATE INDEX [IX_SqlServerOperations_CreatedDate] ON [SqlServerOperations] ([CreatedDate]);

CREATE INDEX [IX_SqlServerOperations_LoginId] ON [SqlServerOperations] ([LoginId]);

CREATE INDEX [IX_SqlServerOperations_RequestId] ON [SqlServerOperations] ([RequestId]);

CREATE INDEX [IX_SqlServerOperations_RoleAssignmentId] ON [SqlServerOperations] ([RoleAssignmentId]);

CREATE INDEX [IX_SqlServerOperations_RoleId] ON [SqlServerOperations] ([RoleId]);

CREATE INDEX [IX_SqlServerOperations_Status] ON [SqlServerOperations] ([Status]);

CREATE INDEX [IX_SqlServerOperations_UserId] ON [SqlServerOperations] ([UserId]);

CREATE INDEX [IX_SqlServerRoleAssignments_LoginId] ON [SqlServerRoleAssignments] ([LoginId]);

CREATE INDEX [IX_SqlServerRoleAssignments_RoleId] ON [SqlServerRoleAssignments] ([RoleId]);

CREATE INDEX [IX_SqlServerRoleAssignments_UserId] ON [SqlServerRoleAssignments] ([UserId]);

CREATE UNIQUE INDEX [IX_SqlServerRoles_RoleName_ServerInstance_DatabaseName] ON [SqlServerRoles] ([RoleName], [ServerInstance], [DatabaseName]) WHERE [DatabaseName] IS NOT NULL;

CREATE INDEX [IX_SqlServerUsers_LoginId] ON [SqlServerUsers] ([LoginId]);

CREATE UNIQUE INDEX [IX_SqlServerUsers_UserName_DatabaseName_ServerInstance] ON [SqlServerUsers] ([UserName], [DatabaseName], [ServerInstance]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250620110432_InitialCreate', N'9.0.5');

COMMIT;
GO

