//namespace IAMBuddy.Tools.Functions;

//using System;
//using System.ComponentModel;
//using System.Linq;
//using System.Threading.Tasks;
//using IAMBuddy.Tools.Data;
//using Microsoft.EntityFrameworkCore;
//using ModelContextProtocol.Server;

//[McpServerToolType]
//public class SqlServerFunctions(IAMBuddyDbContext dbContext)
//{
//    private readonly IAMBuddyDbContext dbContext = dbContext;

//    [McpServerTool, Description("Creates a new SQL Server login account.")]
//    public async Task<string> CreateSqlLogin(
//        [Description("The name of the SQL Server where the login will be created.")] string serverName,
//        [Description("The name of the new login account.")] string loginName,
//        [Description("The type of authentication for the login (SqlServer, Windows, or AzureAD).")] AuthenticationType authenticationType,
//        [Description("The status of the login (Active, Disabled, Locked, Expired). Defaults to Active.")] LoginStatus status = LoginStatus.Active,
//        [Description("The default database for the login.")] string? defaultDatabase = null,
//        [Description("The name of the Active Directory account for Windows or AzureAD authentication (e.g., 'DOMAIN\\username' or 'user@domain.com').")] string? adAccountName = null,
//        [Description("The name of the Active Directory group for Windows or AzureAD authentication (e.g., 'DOMAIN\\groupname' or 'group@domain.com').")] string? adGroupName = null,
//        [Description("The ID of the non-human identity associated with this login, if applicable.")] int? nonHumanIdentityId = null,
//        [Description("A description for the SQL login.")] string? description = null,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            int? activeDirectoryAccountId = null;
//            int? activeDirectoryGroupId = null;

//            if (authenticationType is AuthenticationType.Windows or AuthenticationType.AzureAD)
//            {
//                if (!string.IsNullOrEmpty(adAccountName))
//                {
//                    var adAccount = await this.dbContext.ActiveDirectoryAccounts.FirstOrDefaultAsync(a => a.UserPrincipalName == adAccountName || a.SamAccountName == adAccountName);
//                    if (adAccount == null)
//                    {
//                        return $"Active Directory account '{adAccountName}' not found. Please ensure the AD account exists in the system.";
//                    }
//                    activeDirectoryAccountId = adAccount.Id;
//                }
//                else if (!string.IsNullOrEmpty(adGroupName))
//                {
//                    var adGroup = await this.dbContext.ActiveDirectoryGroups.FirstOrDefaultAsync(g => g.Name == adGroupName || g.SamAccountName == adGroupName);
//                    if (adGroup == null)
//                    {
//                        return $"Active Directory group '{adGroupName}' not found. Please ensure the AD group exists in the system.";
//                    }
//                    activeDirectoryGroupId = adGroup.Id;
//                }
//                else
//                {
//                    return "For Windows or AzureAD authentication, either an Active Directory account name or group name must be provided.";
//                }
//            }

//            var newLogin = new ServerLogin
//            {
//                Name = loginName,
//                ServerId = server.Id,
//                AuthenticationType = authenticationType,
//                Status = status,
//                DefaultDatabase = defaultDatabase,
//                ActiveDirectoryAccountId = activeDirectoryAccountId,
//                ActiveDirectoryGroupId = activeDirectoryGroupId,
//                NonHumanIdentityId = nonHumanIdentityId,
//                Description = description,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.ServerLogins.Add(newLogin);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "ServerLogin",
//                EntityId = newLogin.Id,
//                NewValues = $"LoginName: {loginName}, Server: {serverName}, AuthenticationType: {authenticationType}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created SQL Server login: {loginName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created SQL login '{loginName}' on server '{serverName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating SQL login: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Creates a new database user within a specific database.")]
//    public async Task<string> CreateDatabaseUser(
//        [Description("The name of the SQL Server where the database resides.")] string serverName,
//        [Description("The name of the database where the user will be created.")] string databaseName,
//        [Description("The name of the new database user.")] string userName,
//        [Description("The name of the existing server login to map to this user, if applicable.")] string? serverLoginName = null,
//        [Description("The default schema for the user.")] string? defaultSchema = null,
//        [Description("A description for the database user.")] string? description = null,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var database = await this.dbContext.Databases
//                .Where(d => d.Name == databaseName && d.Instance.ServerId == server.Id)
//                .FirstOrDefaultAsync();

//            if (database == null)
//            {
//                return $"Database '{databaseName}' not found on server '{serverName}'.";
//            }

//            int? serverLoginId = null;
//            if (!string.IsNullOrEmpty(serverLoginName))
//            {
//                var login = await this.dbContext.ServerLogins
//                    .FirstOrDefaultAsync(sl => sl.Name == serverLoginName && sl.ServerId == server.Id);
//                if (login == null)
//                {
//                    return $"Server login '{serverLoginName}' not found on server '{serverName}'. Please create the login first.";
//                }
//                serverLoginId = login.Id;
//            }

//            var newUser = new DatabaseUser
//            {
//                Name = userName,
//                DatabaseId = database.Id,
//                ServerLoginId = serverLoginId,
//                DefaultSchema = defaultSchema,
//                UserType = serverLoginId.HasValue ? "SQL_USER" : "SQL_USER_WITHOUT_LOGIN", // Simplified
//                IsActive = true,
//                Description = description,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.DatabaseUsers.Add(newUser);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "DatabaseUser",
//                EntityId = newUser.Id,
//                NewValues = $"UserName: {userName}, Database: {databaseName}, Server: {serverName}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created database user: {userName} in database {databaseName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created database user '{userName}' in database '{databaseName}' on server '{serverName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating database user: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Grants permissions on a securable to a principal (user or role).")]
//    public async Task<string> GrantPermission(
//        [Description("The name of the SQL Server.")] string serverName,
//        [Description("The name of the securable (e.g., 'MyTable', 'MySchema').")] string securableName,
//        [Description("The permission to grant (e.g., 'SELECT', 'EXECUTE', 'ALTER').")] string permissionName,
//        [Description("The type of securable (Server, Database, Schema, Table, View, StoredProcedure, Function, Column).")] SecurableType securableType,
//        [Description("The name of the database where the securable resides (optional, for database-level securables).")] string? databaseName = null,
//        [Description("The name of the database user to grant permission to (optional).")] string? databaseUserName = null,
//        [Description("The name of the database role to grant permission to (optional).")] string? databaseRoleName = null,
//        [Description("The name of the server role to grant permission to (optional).")] string? serverRoleName = null,
//        [Description("A description for the permission grant.")] string? description = null,
//        [Description("The name of the user performing the action.")] string grantedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            int? databaseId = null;
//            if (!string.IsNullOrEmpty(databaseName))
//            {
//                var database = await this.dbContext.Databases
//                    .Where(d => d.Name == databaseName && d.Instance.ServerId == server.Id)
//                    .FirstOrDefaultAsync();
//                if (database == null)
//                {
//                    return $"Database '{databaseName}' not found on server '{serverName}'.";
//                }
//                databaseId = database.Id;
//            }

//            int? databaseUserId = null;
//            if (!string.IsNullOrEmpty(databaseUserName))
//            {
//                if (!databaseId.HasValue)
//                {
//                    return "Database name is required when granting permission to a database user.";
//                }
//                var dbUser = await this.dbContext.DatabaseUsers
//                    .FirstOrDefaultAsync(u => u.Name == databaseUserName && u.DatabaseId == databaseId.Value);
//                if (dbUser == null)
//                {
//                    return $"Database user '{databaseUserName}' not found in database '{databaseName}'.";
//                }
//                databaseUserId = dbUser.Id;
//            }

//            int? databaseRoleId = null;
//            if (!string.IsNullOrEmpty(databaseRoleName))
//            {
//                if (!databaseId.HasValue)
//                {
//                    return "Database name is required when granting permission to a database role.";
//                }
//                var dbRole = await this.dbContext.DatabaseRoles
//                    .FirstOrDefaultAsync(r => r.Name == databaseRoleName && r.DatabaseId == databaseId.Value);
//                if (dbRole == null)
//                {
//                    return $"Database role '{databaseRoleName}' not found in database '{databaseName}'.";
//                }
//                databaseRoleId = dbRole.Id;
//            }

//            int? serverRoleId = null;
//            if (!string.IsNullOrEmpty(serverRoleName))
//            {
//                var srvRole = await this.dbContext.ServerRoles
//                    .FirstOrDefaultAsync(r => r.Name == serverRoleName && r.ServerId == server.Id);
//                if (srvRole == null)
//                {
//                    return $"Server role '{serverRoleName}' not found on server '{serverName}'.";
//                }
//                serverRoleId = srvRole.Id;
//            }

//            if (!databaseUserId.HasValue && !databaseRoleId.HasValue && !serverRoleId.HasValue)
//            {
//                return "At least one principal (database user, database role, or server role) must be specified to grant permissions.";
//            }

//            var newPermission = new DatabasePermission
//            {
//                PermissionName = permissionName,
//                Type = PermissionType.Grant,
//                SecurableType = securableType,
//                SecurableName = securableName,
//                DatabaseId = databaseId,
//                DatabaseUserId = databaseUserId,
//                DatabaseRoleId = databaseRoleId,
//                ServerRoleId = serverRoleId,
//                GrantedBy = grantedBy,
//                GrantedDate = DateTime.UtcNow,
//                Description = description,
//                CreatedBy = grantedBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.Permissions.Add(newPermission);
//            await this.dbContext.SaveChangesAsync();

//            var principalInfo = "";
//            if (databaseUserId.HasValue)
//            {
//                principalInfo = $"user '{databaseUserName}'";
//            }
//            else if (databaseRoleId.HasValue)
//            {
//                principalInfo = $"database role '{databaseRoleName}'";
//            }
//            else if (serverRoleId.HasValue)
//            {
//                principalInfo = $"server role '{serverRoleName}'";
//            }

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Grant Permission",
//                EntityType = "Permission",
//                EntityId = newPermission.Id,
//                NewValues = $"Permission: {permissionName}, Securable: {securableType}.{securableName}, Principal: {principalInfo}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = grantedBy,
//                Description = $"Granted '{permissionName}' on '{securableType}' '{securableName}' to {principalInfo}."
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully granted '{permissionName}' on '{securableType}' '{securableName}' to {principalInfo}.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error granting permission: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Revokes previously granted permissions on a securable from a principal (user or role).")]
//    public async Task<string> RevokePermission(
//        [Description("The name of the SQL Server.")] string serverName,
//        [Description("The type of securable (Server, Database, Schema, Table, View, StoredProcedure, Function, Column).")] SecurableType securableType,
//        [Description("The name of the securable (e.g., 'MyTable', 'MySchema').")] string securableName,
//        [Description("The permission to revoke (e.g., 'SELECT', 'EXECUTE', 'ALTER').")] string permissionName,
//        [Description("The name of the database where the securable resides (optional, for database-level securables).")] string? databaseName = null,
//        [Description("The name of the database user to revoke permission from (optional).")] string? databaseUserName = null,
//        [Description("The name of the database role to revoke permission from (optional).")] string? databaseRoleName = null,
//        [Description("The name of the server role to revoke permission from (optional).")] string? serverRoleName = null,
//        [Description("The name of the user performing the action.")] string revokedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            int? databaseId = null;
//            if (!string.IsNullOrEmpty(databaseName))
//            {
//                var database = await this.dbContext.Databases
//                    .Where(d => d.Name == databaseName && d.Instance.ServerId == server.Id)
//                    .FirstOrDefaultAsync();
//                if (database == null)
//                {
//                    return $"Database '{databaseName}' not found on server '{serverName}'.";
//                }
//                databaseId = database.Id;
//            }

//            int? databaseUserId = null;
//            if (!string.IsNullOrEmpty(databaseUserName))
//            {
//                if (!databaseId.HasValue)
//                {
//                    return "Database name is required when revoking permission from a database user.";
//                }
//                var dbUser = await this.dbContext.DatabaseUsers
//                    .FirstOrDefaultAsync(u => u.Name == databaseUserName && u.DatabaseId == databaseId.Value);
//                if (dbUser == null)
//                {
//                    return $"Database user '{databaseUserName}' not found in database '{databaseName}'.";
//                }
//                databaseUserId = dbUser.Id;
//            }

//            int? databaseRoleId = null;
//            if (!string.IsNullOrEmpty(databaseRoleName))
//            {
//                if (!databaseId.HasValue)
//                {
//                    return "Database name is required when revoking permission from a database role.";
//                }
//                var dbRole = await this.dbContext.DatabaseRoles
//                    .FirstOrDefaultAsync(r => r.Name == databaseRoleName && r.DatabaseId == databaseId.Value);
//                if (dbRole == null)
//                {
//                    return $"Database role '{databaseRoleName}' not found in database '{databaseName}'.";
//                }
//                databaseRoleId = dbRole.Id;
//            }

//            int? serverRoleId = null;
//            if (!string.IsNullOrEmpty(serverRoleName))
//            {
//                var srvRole = await this.dbContext.ServerRoles
//                    .FirstOrDefaultAsync(r => r.Name == serverRoleName && r.ServerId == server.Id);
//                if (srvRole == null)
//                {
//                    return $"Server role '{serverRoleName}' not found on server '{serverName}'.";
//                }
//                serverRoleId = srvRole.Id;
//            }

//            if (!databaseUserId.HasValue && !databaseRoleId.HasValue && !serverRoleId.HasValue)
//            {
//                return "At least one principal (database user, database role, or server role) must be specified to revoke permissions.";
//            }

//            var permissionToRevoke = await this.dbContext.Permissions
//                .Where(p => p.PermissionName == permissionName &&
//                            p.Type == PermissionType.Grant && // Only revoke previously granted permissions
//                            p.SecurableType == securableType &&
//                            p.SecurableName == securableName &&
//                            p.DatabaseId == databaseId &&
//                            p.DatabaseUserId == databaseUserId &&
//                            p.DatabaseRoleId == databaseRoleId &&
//                            p.ServerRoleId == serverRoleId)
//                .FirstOrDefaultAsync();

//            if (permissionToRevoke == null)
//            {
//                var principalInfoNotFound = "";
//                if (databaseUserId.HasValue)
//                {
//                    principalInfoNotFound = $"user '{databaseUserName}'";
//                }
//                else if (databaseRoleId.HasValue)
//                {
//                    principalInfoNotFound = $"database role '{databaseRoleName}'";
//                }
//                else if (serverRoleId.HasValue)
//                {
//                    principalInfoNotFound = $"server role '{serverRoleName}'";
//                }

//                return $"Permission '{permissionName}' on '{securableType}' '{securableName}' was not found as granted to {principalInfoNotFound}.";
//            }

//            this.dbContext.Permissions.Remove(permissionToRevoke);
//            await this.dbContext.SaveChangesAsync();

//            var principalInfo = "";
//            if (databaseUserId.HasValue)
//            {
//                principalInfo = $"user '{databaseUserName}'";
//            }
//            else if (databaseRoleId.HasValue)
//            {
//                principalInfo = $"database role '{databaseRoleName}'";
//            }
//            else if (serverRoleId.HasValue)
//            {
//                principalInfo = $"server role '{serverRoleName}'";
//            }

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Revoke Permission",
//                EntityType = "Permission",
//                EntityId = permissionToRevoke.Id,
//                OldValues = $"Permission: {permissionName}, Securable: {securableType}.{securableName}, Principal: {principalInfo}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = revokedBy,
//                Description = $"Revoked '{permissionName}' on '{securableType}' '{securableName}' from {principalInfo}."
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully revoked '{permissionName}' on '{securableType}' '{securableName}' from {principalInfo}.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error revoking permission: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Modifies the properties of an existing SQL Server login account.")]
//    public async Task<string> AlterSqlLogin(
//        [Description("The name of the SQL Server where the login exists.")] string serverName,
//        [Description("The current name of the login account to modify.")] string currentLoginName,
//        [Description("The new name for the login account (optional, if renaming).")] string? newLoginName = null,
//        [Description("The new status for the login (Active, Disabled, Locked, Expired). Optional.")] LoginStatus? newStatus = null,
//        [Description("The new default database for the login. Optional.")] string? newDefaultDatabase = null,
//        [Description("The new default language for the login. Optional.")] string? newDefaultLanguage = null,
//        [Description("The name of the user performing the action.")] string modifiedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var login = await this.dbContext.ServerLogins
//                .FirstOrDefaultAsync(sl => sl.Name == currentLoginName && sl.ServerId == server.Id);

//            if (login == null)
//            {
//                return $"SQL login '{currentLoginName}' not found on server '{serverName}'.";
//            }

//            var oldValues = $"LoginName: {login.Name}, Status: {login.Status}, DefaultDatabase: {login.DefaultDatabase}, DefaultLanguage: {login.DefaultLanguage}";

//            var changesMade = false;
//            if (!string.IsNullOrEmpty(newLoginName) && login.Name != newLoginName)
//            {
//                login.Name = newLoginName;
//                changesMade = true;
//            }
//            if (newStatus.HasValue && login.Status != newStatus.Value)
//            {
//                login.Status = newStatus.Value;
//                changesMade = true;
//            }
//            if (!string.IsNullOrEmpty(newDefaultDatabase) && login.DefaultDatabase != newDefaultDatabase)
//            {
//                login.DefaultDatabase = newDefaultDatabase;
//                changesMade = true;
//            }
//            if (!string.IsNullOrEmpty(newDefaultLanguage) && login.DefaultLanguage != newDefaultLanguage)
//            {
//                login.DefaultLanguage = newDefaultLanguage;
//                changesMade = true;
//            }

//            if (!changesMade)
//            {
//                return $"No changes were provided to alter SQL login '{currentLoginName}'.";
//            }

//            login.ModifiedBy = modifiedBy;
//            login.ModifiedDate = DateTime.UtcNow;

//            await this.dbContext.SaveChangesAsync();

//            var newValues = $"LoginName: {login.Name}, Status: {login.Status}, DefaultDatabase: {login.DefaultDatabase}, DefaultLanguage: {login.DefaultLanguage}";

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Alter",
//                EntityType = "ServerLogin",
//                EntityId = login.Id,
//                OldValues = oldValues,
//                NewValues = newValues,
//                ActionDate = DateTime.UtcNow,
//                ActionBy = modifiedBy,
//                Description = $"Altered SQL Server login: {currentLoginName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully altered SQL login '{currentLoginName}' on server '{serverName}'.{(string.IsNullOrEmpty(newLoginName) ? "" : $" New name: '{newLoginName}'.")}";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error altering SQL login: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Modifies the properties of an existing database user.")]
//    public async Task<string> AlterDatabaseUser(
//        [Description("The name of the SQL Server where the database resides.")] string serverName,
//        [Description("The name of the database where the user exists.")] string databaseName,
//        [Description("The current name of the database user to modify.")] string currentUserName,
//        [Description("The new name for the database user (optional, if renaming).")] string? newUserName = null,
//        [Description("The new name of the server login to map this user to (optional, if remapping).")] string? newServerLoginName = null,
//        [Description("The new default schema for the user. Optional.")] string? newDefaultSchema = null,
//        [Description("The name of the user performing the action.")] string modifiedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var database = await this.dbContext.Databases
//                .Where(d => d.Name == databaseName && d.Instance.ServerId == server.Id)
//                .FirstOrDefaultAsync();

//            if (database == null)
//            {
//                return $"Database '{databaseName}' not found on server '{serverName}'.";
//            }

//            var user = await this.dbContext.DatabaseUsers
//                .Include(u => u.ServerLogin)
//                .FirstOrDefaultAsync(u => u.Name == currentUserName && u.DatabaseId == database.Id);

//            if (user == null)
//            {
//                return $"Database user '{currentUserName}' not found in database '{databaseName}' on server '{serverName}'.";
//            }

//            var oldValues = $"UserName: {user.Name}, ServerLogin: {user.ServerLogin?.Name}, DefaultSchema: {user.DefaultSchema}";

//            var changesMade = false;
//            if (!string.IsNullOrEmpty(newUserName) && user.Name != newUserName)
//            {
//                user.Name = newUserName;
//                changesMade = true;
//            }
//            if (!string.IsNullOrEmpty(newServerLoginName))
//            {
//                var newLogin = await this.dbContext.ServerLogins
//                    .FirstOrDefaultAsync(sl => sl.Name == newServerLoginName && sl.ServerId == server.Id);
//                if (newLogin == null)
//                {
//                    return $"New server login '{newServerLoginName}' not found on server '{serverName}'.";
//                }
//                if (user.ServerLoginId != newLogin.Id)
//                {
//                    user.ServerLoginId = newLogin.Id;
//                    changesMade = true;
//                }
//            }
//            if (!string.IsNullOrEmpty(newDefaultSchema) && user.DefaultSchema != newDefaultSchema)
//            {
//                user.DefaultSchema = newDefaultSchema;
//                changesMade = true;
//            }

//            if (!changesMade)
//            {
//                return $"No changes were provided to alter database user '{currentUserName}'.";
//            }

//            user.ModifiedBy = modifiedBy;
//            user.ModifiedDate = DateTime.UtcNow;

//            await this.dbContext.SaveChangesAsync();

//            var newValues = $"UserName: {user.Name}, ServerLogin: {user.ServerLogin?.Name}, DefaultSchema: {user.DefaultSchema}";

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Alter",
//                EntityType = "DatabaseUser",
//                EntityId = user.Id,
//                OldValues = oldValues,
//                NewValues = newValues,
//                ActionDate = DateTime.UtcNow,
//                ActionBy = modifiedBy,
//                Description = $"Altered database user: {currentUserName} in database {databaseName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully altered database user '{currentUserName}' in database '{databaseName}' on server '{serverName}'.{(string.IsNullOrEmpty(newUserName) ? "" : $" New name: '{newUserName}'.")}";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error altering database user: {ex.Message}";
//        }
//    }


//    [McpServerTool, Description("Deletes a SQL Server login account.")]
//    public async Task<string> DropSqlLogin(
//        [Description("The name of the SQL Server where the login exists.")] string serverName,
//        [Description("The name of the login account to delete.")] string loginName,
//        [Description("The name of the user performing the action.")] string droppedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var login = await this.dbContext.ServerLogins
//                .Include(sl => sl.DatabaseUsers) // Include dependent users
//                .FirstOrDefaultAsync(sl => sl.Name == loginName && sl.ServerId == server.Id);

//            if (login == null)
//            {
//                return $"SQL login '{loginName}' not found on server '{serverName}'.";
//            }

//            if (login.DatabaseUsers.Count != 0)
//            {
//                return $"Cannot drop login '{loginName}' because it is mapped to existing database users. Please drop or remap those users first.";
//            }

//            this.dbContext.ServerLogins.Remove(login);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Drop",
//                EntityType = "ServerLogin",
//                EntityId = login.Id,
//                OldValues = $"LoginName: {loginName}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = droppedBy,
//                Description = $"Dropped SQL Server login: {loginName} from server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully dropped SQL login '{loginName}' from server '{serverName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error dropping SQL login: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Deletes a database user from a specific database.")]
//    public async Task<string> DropDatabaseUser(
//        [Description("The name of the SQL Server where the database resides.")] string serverName,
//        [Description("The name of the database where the user exists.")] string databaseName,
//        [Description("The name of the database user to delete.")] string userName,
//        [Description("The name of the user performing the action.")] string droppedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var database = await this.dbContext.Databases
//                .Where(d => d.Name == databaseName && d.Instance.ServerId == server.Id)
//                .FirstOrDefaultAsync();

//            if (database == null)
//            {
//                return $"Database '{databaseName}' not found on server '{serverName}'.";
//            }

//            var user = await this.dbContext.DatabaseUsers
//                .Include(u => u.DatabaseUserRoles) // Include dependent roles
//                .FirstOrDefaultAsync(u => u.Name == userName && u.DatabaseId == database.Id);

//            if (user == null)
//            {
//                return $"Database user '{userName}' not found in database '{databaseName}' on server '{serverName}'.";
//            }

//            if (user.DatabaseUserRoles.Count != 0)
//            {
//                return $"Cannot drop database user '{userName}' because it is a member of existing database roles. Please remove the user from those roles first.";
//            }

//            this.dbContext.DatabaseUsers.Remove(user);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Drop",
//                EntityType = "DatabaseUser",
//                EntityId = user.Id,
//                OldValues = $"UserName: {userName}, Database: {databaseName}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = droppedBy,
//                Description = $"Dropped database user: {userName} from database {databaseName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully dropped database user '{userName}' from database '{databaseName}' on server '{serverName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error dropping database user: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Creates a new SQL Server role.")]
//    public async Task<string> CreateSqlServerRole(
//        [Description("The name of the SQL Server where the role will be created.")] string serverName,
//        [Description("The name of the new server role.")] string roleName,
//        [Description("Set to 'true' if this is a fixed server role, 'false' for user-defined. Defaults to 'false'.")] bool isFixedRole = false,
//        [Description("A description for the server role.")] string? description = null,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var newServerRole = new ServerRole
//            {
//                Name = roleName,
//                ServerId = server.Id,
//                Type = RoleType.ServerRole, // Assuming all created roles are ServerRole type
//                IsFixedRole = isFixedRole,
//                Description = description,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.ServerRoles.Add(newServerRole);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "ServerRole",
//                EntityId = newServerRole.Id,
//                NewValues = $"RoleName: {roleName}, Server: {serverName}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created SQL Server role: {roleName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created SQL Server role '{roleName}' on server '{serverName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating SQL Server role: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Creates a new database role within a specific database.")]
//    public async Task<string> CreateDatabaseRole(
//        [Description("The name of the SQL Server where the database resides.")] string serverName,
//        [Description("The name of the database where the role will be created.")] string databaseName,
//        [Description("The name of the new database role.")] string roleName,
//        [Description("Set to 'true' if this is a fixed database role, 'false' for user-defined. Defaults to 'false'.")] bool isFixedRole = false,
//        [Description("A description for the database role.")] string? description = null,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var database = await this.dbContext.Databases
//                .Where(d => d.Name == databaseName && d.Instance.ServerId == server.Id)
//                .FirstOrDefaultAsync();

//            if (database == null)
//            {
//                return $"Database '{databaseName}' not found on server '{serverName}'.";
//            }

//            var newDatabaseRole = new DatabaseRole
//            {
//                Name = roleName,
//                DatabaseId = database.Id,
//                Type = RoleType.DatabaseRole, // Assuming all created roles are DatabaseRole type
//                IsFixedRole = isFixedRole,
//                Description = description,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.DatabaseRoles.Add(newDatabaseRole);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "DatabaseRole",
//                EntityId = newDatabaseRole.Id,
//                NewValues = $"RoleName: {roleName}, Database: {databaseName}, Server: {serverName}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created database role: {roleName} in database {databaseName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created database role '{roleName}' in database '{databaseName}' on server '{serverName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating database role: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Modifies an existing SQL Server role (rename, add member, or remove member).")]
//    public async Task<string> AlterSqlServerRole(
//        [Description("The name of the SQL Server where the role exists.")] string serverName,
//        [Description("The current name of the server role to modify.")] string currentRoleName,
//        [Description("The new name for the server role (optional, if renaming).")] string? newRoleName = null,
//        [Description("The name of the server login to add as a member (optional).")] string? loginToAdd = null,
//        [Description("The name of the server login to remove as a member (optional).")] string? loginToRemove = null,
//        [Description("The name of the user performing the action.")] string modifiedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var serverRole = await this.dbContext.ServerRoles
//                .Include(sr => sr.ServerLoginRoles)
//                .ThenInclude(slr => slr.ServerLogin)
//                .FirstOrDefaultAsync(sr => sr.Name == currentRoleName && sr.ServerId == server.Id);

//            if (serverRole == null)
//            {
//                return $"SQL Server role '{currentRoleName}' not found on server '{serverName}'.";
//            }

//            var oldValues = $"RoleName: {serverRole.Name}, Members: [{string.Join(", ", serverRole.ServerLoginRoles.Select(slr => slr.ServerLogin.Name))}]";
//            var changesMade = false;

//            if (!string.IsNullOrEmpty(newRoleName) && serverRole.Name != newRoleName)
//            {
//                serverRole.Name = newRoleName;
//                changesMade = true;
//            }

//            if (!string.IsNullOrEmpty(loginToAdd))
//            {
//                var login = await this.dbContext.ServerLogins.FirstOrDefaultAsync(sl => sl.Name == loginToAdd && sl.ServerId == server.Id);
//                if (login == null)
//                {
//                    return $"Server login '{loginToAdd}' not found on server '{serverName}'.";
//                }
//                if (!serverRole.ServerLoginRoles.Any(slr => slr.ServerLoginId == login.Id))
//                {
//                    serverRole.ServerLoginRoles.Add(new ServerLoginRole
//                    {
//                        ServerLoginId = login.Id,
//                        ServerRoleId = serverRole.Id,
//                        AssignedDate = DateTime.UtcNow,
//                        AssignedBy = modifiedBy,
//                        CreatedBy = modifiedBy,
//                        CreatedDate = DateTime.UtcNow
//                    });
//                    changesMade = true;
//                }
//                else
//                {
//                    return $"Login '{loginToAdd}' is already a member of server role '{currentRoleName}'.";
//                }
//            }

//            if (!string.IsNullOrEmpty(loginToRemove))
//            {
//                var loginRoleToRemove = serverRole.ServerLoginRoles.FirstOrDefault(slr => slr.ServerLogin.Name == loginToRemove);
//                if (loginRoleToRemove != null)
//                {
//                    this.dbContext.ServerLoginRoles.Remove(loginRoleToRemove);
//                    changesMade = true;
//                }
//                else
//                {
//                    return $"Login '{loginToRemove}' is not a member of server role '{currentRoleName}'.";
//                }
//            }

//            if (!changesMade)
//            {
//                return $"No changes were provided to alter SQL Server role '{currentRoleName}'.";
//            }

//            serverRole.ModifiedBy = modifiedBy;
//            serverRole.ModifiedDate = DateTime.UtcNow;

//            await this.dbContext.SaveChangesAsync();

//            var newMembers = await this.dbContext.ServerLoginRoles
//                .Where(slr => slr.ServerRoleId == serverRole.Id)
//                .Select(slr => slr.ServerLogin.Name)
//                .ToListAsync();
//            var newValues = $"RoleName: {serverRole.Name}, Members: [{string.Join(", ", newMembers)}]";


//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Alter",
//                EntityType = "ServerRole",
//                EntityId = serverRole.Id,
//                OldValues = oldValues,
//                NewValues = newValues,
//                ActionDate = DateTime.UtcNow,
//                ActionBy = modifiedBy,
//                Description = $"Altered SQL Server role: {currentRoleName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully altered SQL Server role '{currentRoleName}' on server '{serverName}'.{(string.IsNullOrEmpty(newRoleName) ? "" : $" New name: '{newRoleName}'.")}";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error altering SQL Server role: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Modifies an existing database role (rename, add member, or remove member).")]
//    public async Task<string> AlterDatabaseRole(
//        [Description("The name of the SQL Server where the database resides.")] string serverName,
//        [Description("The name of the database where the role exists.")] string databaseName,
//        [Description("The current name of the database role to modify.")] string currentRoleName,
//        [Description("The new name for the database role (optional, if renaming).")] string? newRoleName = null,
//        [Description("The name of the database user to add as a member (optional).")] string? userToAdd = null,
//        [Description("The name of the database user to remove as a member (optional).")] string? userToRemove = null,
//        [Description("The name of the user performing the action.")] string modifiedBy = "System"
//    )
//    {
//        try
//        {
//            var server = await this.dbContext.SqlServers.FirstOrDefaultAsync(s => s.Name == serverName);
//            if (server == null)
//            {
//                return $"SQL Server '{serverName}' not found.";
//            }

//            var database = await this.dbContext.Databases
//                .Where(d => d.Name == databaseName && d.Instance.ServerId == server.Id)
//                .FirstOrDefaultAsync();

//            if (database == null)
//            {
//                return $"Database '{databaseName}' not found on server '{serverName}'.";
//            }

//            var databaseRole = await this.dbContext.DatabaseRoles
//                .Include(dr => dr.DatabaseUserRoles)
//                .ThenInclude(dur => dur.DatabaseUser)
//                .FirstOrDefaultAsync(dr => dr.Name == currentRoleName && dr.DatabaseId == database.Id);

//            if (databaseRole == null)
//            {
//                return $"Database role '{currentRoleName}' not found in database '{databaseName}' on server '{serverName}'.";
//            }

//            var oldMembers = databaseRole.DatabaseUserRoles.Select(dur => dur.DatabaseUser.Name).ToList();
//            var oldValues = $"RoleName: {databaseRole.Name}, Members: [{string.Join(", ", oldMembers)}]";
//            var changesMade = false;

//            if (!string.IsNullOrEmpty(newRoleName) && databaseRole.Name != newRoleName)
//            {
//                databaseRole.Name = newRoleName;
//                changesMade = true;
//            }

//            if (!string.IsNullOrEmpty(userToAdd))
//            {
//                var user = await this.dbContext.DatabaseUsers.FirstOrDefaultAsync(u => u.Name == userToAdd && u.DatabaseId == database.Id);
//                if (user == null)
//                {
//                    return $"Database user '{userToAdd}' not found in database '{databaseName}'.";
//                }
//                if (!databaseRole.DatabaseUserRoles.Any(dur => dur.DatabaseUserId == user.Id))
//                {
//                    databaseRole.DatabaseUserRoles.Add(new DatabaseUserRole
//                    {
//                        DatabaseUserId = user.Id,
//                        DatabaseRoleId = databaseRole.Id,
//                        AssignedDate = DateTime.UtcNow,
//                        AssignedBy = modifiedBy,
//                        CreatedBy = modifiedBy,
//                        CreatedDate = DateTime.UtcNow
//                    });
//                    changesMade = true;
//                }
//                else
//                {
//                    return $"User '{userToAdd}' is already a member of database role '{currentRoleName}'.";
//                }
//            }

//            if (!string.IsNullOrEmpty(userToRemove))
//            {
//                var userRoleToRemove = databaseRole.DatabaseUserRoles.FirstOrDefault(dur => dur.DatabaseUser.Name == userToRemove);
//                if (userRoleToRemove != null)
//                {
//                    this.dbContext.DatabaseUserRoles.Remove(userRoleToRemove);
//                    changesMade = true;
//                }
//                else
//                {
//                    return $"User '{userToRemove}' is not a member of database role '{currentRoleName}'.";
//                }
//            }

//            if (!changesMade)
//            {
//                return $"No changes were provided to alter database role '{currentRoleName}'.";
//            }

//            databaseRole.ModifiedBy = modifiedBy;
//            databaseRole.ModifiedDate = DateTime.UtcNow;

//            await this.dbContext.SaveChangesAsync();

//            var newMembers = await this.dbContext.DatabaseUserRoles
//                .Where(dur => dur.DatabaseRoleId == databaseRole.Id)
//                .Select(dur => dur.DatabaseUser.Name)
//                .ToListAsync();
//            var newValues = $"RoleName: {databaseRole.Name}, Members: [{string.Join(", ", newMembers)}]";

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Alter",
//                EntityType = "DatabaseRole",
//                EntityId = databaseRole.Id,
//                OldValues = oldValues,
//                NewValues = newValues,
//                ActionDate = DateTime.UtcNow,
//                ActionBy = modifiedBy,
//                Description = $"Altered database role: {currentRoleName} in database {databaseName} on server {serverName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully altered database role '{currentRoleName}' in database '{databaseName}' on server '{serverName}'.{(string.IsNullOrEmpty(newRoleName) ? "" : $" New name: '{newRoleName}'.")}";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error altering database role: {ex.Message}";
//        }
//    }
//}
