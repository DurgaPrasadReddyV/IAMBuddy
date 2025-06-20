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
CREATE TABLE [AccountRequests] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(100) NOT NULL,
    [DatabaseName] nvarchar(100) NOT NULL,
    [ServerName] nvarchar(100) NOT NULL,
    [RequestorEmail] nvarchar(200) NOT NULL,
    [BusinessJustification] nvarchar(1000) NOT NULL,
    [RequestedDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [Status] int NOT NULL,
    [ApproverEmail] nvarchar(max) NULL,
    [WorkflowId] nvarchar(450) NULL,
    CONSTRAINT [PK_AccountRequests] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_AccountRequests_RequestedDate] ON [AccountRequests] ([RequestedDate]);

CREATE INDEX [IX_AccountRequests_RequestorEmail] ON [AccountRequests] ([RequestorEmail]);

CREATE INDEX [IX_AccountRequests_Status] ON [AccountRequests] ([Status]);

CREATE INDEX [IX_AccountRequests_Username_Server_Database] ON [AccountRequests] ([Username], [ServerName], [DatabaseName]);

CREATE INDEX [IX_AccountRequests_WorkflowId] ON [AccountRequests] ([WorkflowId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250620110502_InitialCreate', N'9.0.5');

COMMIT;
GO

