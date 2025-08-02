//namespace IAMBuddy.Tools.Functions;

//using System;
//using System.ComponentModel;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using ModelContextProtocol.Server;

//// Placeholder for your actual DbContext

//[McpServerToolType]
//public class ActiveDirectoryFunctions(ToolsDbContext dbContext)
//{
//    private readonly ToolsDbContext dbContext = dbContext;

//    [McpServerTool, Description("Creates a new Active Directory user account.")]
//    public async Task<string> CreateActiveDirectoryUserAccount(
//        [Description("The SAM account name for the new user account (e.g., 'jdoe').")] string samAccountName,
//        [Description("The display name for the new user account (e.g., 'John Doe').")] string displayName,
//        [Description("The user principal name for the new user account (e.g., 'jdoe@contoso.com').")] string userPrincipalName,
//        [Description("A description for the user account.")] string? description = null,
//        [Description("The domain of the Active Directory account (e.g., 'contoso.com').")] string domain = "yourcompany.com",
//        [Description("Set to 'true' if the account should be enabled, 'false' otherwise. Defaults to 'true'.")] bool isEnabled = true,
//        [Description("Set to 'true' if the password should never expire. Defaults to 'false'.")] bool passwordNeverExpires = false,
//        [Description("Set to 'true' if the user cannot change their password. Defaults to 'false'.")] bool userCannotChangePassword = false,
//        [Description("The ID of the associated human identity, if applicable.")] int? humanIdentityId = null,
//        [Description("The ID of the associated non-human identity (e.g., service account), if applicable.")] int? nonHumanIdentityId = null,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var newAccount = new ActiveDirectoryAccount
//            {
//                SamAccountName = samAccountName,
//                DisplayName = displayName,
//                UserPrincipalName = userPrincipalName,
//                Description = description,
//                AccountType = ActiveDirectoryAccountType.User,
//                Domain = domain,
//                DistinguishedName = $"CN={displayName},CN=Users,DC={domain.Replace(".", ",DC=")}", // Simple DN generation
//                IsEnabled = isEnabled,
//                PasswordNeverExpires = passwordNeverExpires,
//                UserCannotChangePassword = userCannotChangePassword,
//                HumanIdentityId = humanIdentityId,
//                NonHumanIdentityId = nonHumanIdentityId,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.ActiveDirectoryAccounts.Add(newAccount);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "ActiveDirectoryAccount",
//                EntityId = newAccount.Id,
//                NewValues = $"SamAccountName: {samAccountName}, DisplayName: {displayName}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created Active Directory user account: {displayName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created Active Directory user account '{displayName}' with SAM account name '{samAccountName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating Active Directory user account: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Creates a new Active Directory service account.")]
//    public async Task<string> CreateActiveDirectoryServiceAccount(
//        [Description("The SAM account name for the new service account (e.g., 'svc_app').")] string samAccountName,
//        [Description("The display name for the new service account (e.g., 'Application Service Account').")] string displayName,
//        [Description("The ID of the associated non-human identity (e.g., service account).")] int nonHumanIdentityId,
//        [Description("The user principal name for the new service account (e.g., 'svc_app@contoso.com').")] string userPrincipalName,
//        [Description("A description for the service account.")] string? description = null,
//        [Description("The domain of the Active Directory account (e.g., 'contoso.com').")] string domain = "yourcompany.com",
//        [Description("Set to 'true' if the account should be enabled, 'false' otherwise. Defaults to 'true'.")] bool isEnabled = true,
//        [Description("Set to 'true' if the password should never expire. Defaults to 'true'.")] bool passwordNeverExpires = true,
//        [Description("Set to 'true' if the user cannot change their password. Defaults to 'true'.")] bool userCannotChangePassword = true,
//        [Description("A comma-separated list of Service Principal Names (SPNs) for the account.")] string? servicePrincipalNames = null,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var nonHumanIdentity = await this.dbContext.NonHumanIdentities.FindAsync(nonHumanIdentityId);
//            if (nonHumanIdentity == null)
//            {
//                return $"Non-human identity with ID {nonHumanIdentityId} not found. Please create the non-human identity first.";
//            }

//            var newAccount = new ActiveDirectoryAccount
//            {
//                SamAccountName = samAccountName,
//                DisplayName = displayName,
//                UserPrincipalName = userPrincipalName,
//                Description = description,
//                AccountType = ActiveDirectoryAccountType.ServiceAccount,
//                Domain = domain,
//                DistinguishedName = $"CN={displayName},CN=Users,DC={domain.Replace(".", ",DC=")}", // Simple DN generation
//                IsEnabled = isEnabled,
//                PasswordNeverExpires = passwordNeverExpires,
//                UserCannotChangePassword = userCannotChangePassword,
//                NonHumanIdentityId = nonHumanIdentityId,
//                ServicePrincipalNames = servicePrincipalNames,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.ActiveDirectoryAccounts.Add(newAccount);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "ActiveDirectoryAccount",
//                EntityId = newAccount.Id,
//                NewValues = $"SamAccountName: {samAccountName}, DisplayName: {displayName}, NonHumanIdentityId: {nonHumanIdentityId}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created Active Directory service account: {displayName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created Active Directory service account '{displayName}' with SAM account name '{samAccountName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating Active Directory service account: {ex.Message}";
//        }
//    }


//    [McpServerTool, Description("Creates a new Active Directory security group.")]
//    public async Task<string> CreateActiveDirectorySecurityGroup(
//        [Description("The name of the new security group.")] string groupName,
//        [Description("The SAM account name for the new security group (e.g., 'SG_AppAdmins').")] string samAccountName,
//        [Description("A description for the group.")] string? description = null,
//        [Description("The scope of the group (e.g., 'DomainLocal', 'Global', 'Universal').")] ActiveDirectoryGroupScope groupScope = ActiveDirectoryGroupScope.Global,
//        [Description("The domain of the Active Directory group (e.g., 'contoso.com').")] string domain = "yourcompany.com",
//        [Description("The ID of the associated non-human identity managing this group, if applicable.")] int? nonHumanIdentityId = null,
//        [Description("Set to 'true' if the group is active, 'false' otherwise. Defaults to 'true'.")] bool isActive = true,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var newGroup = new ActiveDirectoryGroup
//            {
//                Name = groupName,
//                SamAccountName = samAccountName,
//                DisplayName = groupName, // Often the same as Name for groups
//                Description = description,
//                GroupType = ActiveDirectoryGroupType.Security,
//                GroupScope = groupScope,
//                Domain = domain,
//                DistinguishedName = $"CN={groupName},CN=Users,DC={domain.Replace(".", ",DC=")}", // Simple DN generation
//                NonHumanIdentityId = nonHumanIdentityId,
//                IsActive = isActive,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.ActiveDirectoryGroups.Add(newGroup);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "ActiveDirectoryGroup",
//                EntityId = newGroup.Id,
//                NewValues = $"Name: {groupName}, SamAccountName: {samAccountName}, GroupScope: {groupScope}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created Active Directory security group: {groupName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created Active Directory security group '{groupName}' with SAM account name '{samAccountName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating Active Directory security group: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Creates a new Active Directory distribution group.")]
//    public async Task<string> CreateActiveDirectoryDistributionGroup(
//        [Description("The name of the new distribution group.")] string groupName,
//        [Description("The SAM account name for the new distribution group (e.g., 'DL_Announcements').")] string samAccountName,
//        [Description("A description for the group.")] string? description = null,
//        [Description("The scope of the group (e.g., 'DomainLocal', 'Global', 'Universal').")] ActiveDirectoryGroupScope groupScope = ActiveDirectoryGroupScope.Global,
//        [Description("The domain of the Active Directory group (e.g., 'contoso.com').")] string domain = "yourcompany.com",
//        [Description("The email address for the distribution group.")] string? email = null,
//        [Description("The ID of the associated non-human identity managing this group, if applicable.")] int? nonHumanIdentityId = null,
//        [Description("Set to 'true' if the group is active, 'false' otherwise. Defaults to 'true'.")] bool isActive = true,
//        [Description("The name of the user performing the action.")] string createdBy = "System"
//    )
//    {
//        try
//        {
//            var newGroup = new ActiveDirectoryGroup
//            {
//                Name = groupName,
//                SamAccountName = samAccountName,
//                DisplayName = groupName, // Often the same as Name for groups
//                Description = description,
//                GroupType = ActiveDirectoryGroupType.Distribution,
//                GroupScope = groupScope,
//                Domain = domain,
//                DistinguishedName = $"CN={groupName},CN=Users,DC={domain.Replace(".", ",DC=")}", // Simple DN generation
//                Email = email,
//                NonHumanIdentityId = nonHumanIdentityId,
//                IsActive = isActive,
//                CreatedBy = createdBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.ActiveDirectoryGroups.Add(newGroup);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Create",
//                EntityType = "ActiveDirectoryGroup",
//                EntityId = newGroup.Id,
//                NewValues = $"Name: {groupName}, SamAccountName: {samAccountName}, GroupType: Distribution, Email: {email}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = createdBy,
//                Description = $"Created Active Directory distribution group: {groupName}"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully created Active Directory distribution group '{groupName}' with SAM account name '{samAccountName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error creating Active Directory distribution group: {ex.Message}";
//        }
//    }

//    [McpServerTool, Description("Adds an Active Directory account (user or group) to an Active Directory group.")]
//    public async Task<string> AddActiveDirectoryGroupMember(
//        [Description("The SAM account name of the group to add the member to.")] string groupSamAccountName,
//        [Description("The SAM account name of the member to add (can be a user account or another group).")] string memberSamAccountName,
//        [Description("Specify 'Account' if the member is a user account, or 'Group' if the member is another group.")] string memberType, // "Account" or "Group"
//        [Description("The name of the user performing the action.")] string addedBy = "System"
//    )
//    {
//        try
//        {
//            var group = await this.dbContext.ActiveDirectoryGroups
//                .FirstOrDefaultAsync(g => g.SamAccountName == groupSamAccountName);

//            if (group == null)
//            {
//                return $"Active Directory group '{groupSamAccountName}' not found.";
//            }

//            int? accountId = null;
//            int? childGroupId = null;

//            if (memberType.Equals("Account", StringComparison.OrdinalIgnoreCase))
//            {
//                var account = await this.dbContext.ActiveDirectoryAccounts
//                    .FirstOrDefaultAsync(a => a.SamAccountName == memberSamAccountName);
//                if (account == null)
//                {
//                    return $"Active Directory account '{memberSamAccountName}' not found.";
//                }
//                accountId = account.Id;
//            }
//            else if (memberType.Equals("Group", StringComparison.OrdinalIgnoreCase))
//            {
//                var childGroup = await this.dbContext.ActiveDirectoryGroups
//                    .FirstOrDefaultAsync(g => g.SamAccountName == memberSamAccountName);
//                if (childGroup == null)
//                {
//                    return $"Active Directory group '{memberSamAccountName}' not found.";
//                }
//                childGroupId = childGroup.Id;
//            }
//            else
//            {
//                return "Invalid member type. Please specify 'Account' or 'Group'.";
//            }

//            var existingMembership = await this.dbContext.ActiveDirectoryGroupMemberships
//                .FirstOrDefaultAsync(m => m.GroupId == group.Id &&
//                                         (m.AccountId == accountId || m.ChildGroupId == childGroupId));

//            if (existingMembership != null)
//            {
//                return $"The {memberType.ToLowerInvariant()} '{memberSamAccountName}' is already a member of group '{groupSamAccountName}'.";
//            }

//            var newMembership = new ActiveDirectoryGroupMembership
//            {
//                GroupId = group.Id,
//                AccountId = accountId,
//                ChildGroupId = childGroupId,
//                MemberSince = DateTime.UtcNow,
//                AddedBy = addedBy,
//                CreatedBy = addedBy,
//                CreatedDate = DateTime.UtcNow
//            };

//            this.dbContext.ActiveDirectoryGroupMemberships.Add(newMembership);
//            await this.dbContext.SaveChangesAsync();

//            await this.dbContext.AdminAuditLogs.AddAsync(new AdminAuditLog
//            {
//                Action = "Add Member",
//                EntityType = "ActiveDirectoryGroup",
//                EntityId = group.Id,
//                NewValues = $"MemberType: {memberType}, MemberSamAccountName: {memberSamAccountName}",
//                ActionDate = DateTime.UtcNow,
//                ActionBy = addedBy,
//                Description = $"Added {memberType.ToLowerInvariant()} '{memberSamAccountName}' to Active Directory group '{groupSamAccountName}'"
//            });
//            await this.dbContext.SaveChangesAsync();

//            return $"Successfully added {memberType.ToLowerInvariant()} '{memberSamAccountName}' to Active Directory group '{groupSamAccountName}'.";
//        }
//        catch (Exception ex)
//        {
//            // Log the exception
//            return $"Error adding member to Active Directory group: {ex.Message}";
//        }
//    }
//}
