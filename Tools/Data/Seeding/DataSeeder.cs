using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class DataSeeder
{
    private readonly ToolsDbContext _context;

    public DataSeeder(ToolsDbContext context)
    {
        this._context = context;
        // Ensure that a consistent seed is used for repeatable results in development/testing
        Randomizer.Seed = new Random(8675309);
    }

    // Helper method to convert dates to UTC
    private static DateTime ToUtc(DateTime? dateTime) => dateTime?.ToUniversalTime() ?? DateTime.UtcNow;

    public async Task SeedAllDummyData(SeedConfiguration config)
    {
        Console.WriteLine("Starting data seeding...");

        // Ensure database is created and migrations are applied
        await this._context.Database.MigrateAsync();

        // Seed core entities first as others depend on them
        var humanIdentities = await this.SeedHumanIdentities(config.HumanIdentityCount);
        var businessApplications = await this.SeedBusinessApplications(config.BusinessApplicationCount, humanIdentities);
        var sqlServers = await this.SeedSqlServers(config.SqlServerCount);
        var sqlServerInstances = await this.SeedSqlServerInstances(sqlServers);
        var databases = await this.SeedDatabases(sqlServerInstances);

        // Seed dependent entities
        await this.SeedBusinessApplicationEnvironments(config.BusinessApplicationEnvironmentCount, businessApplications);
        await this.SeedBusinessApplicationTeamMembers(config.BusinessApplicationTeamMemberCount, businessApplications, humanIdentities);
        await this.SeedNonHumanIdentities(config.NonHumanIdentityCount, businessApplications, humanIdentities, [.. this._context.BusinessApplicationEnvironments]); // Pass actual environments
        await this.SeedActiveDirectoryAccounts(config.ActiveDirectoryAccountCount, humanIdentities, [.. this._context.NonHumanIdentities]); // Pass actual non-human identities
        await this.SeedActiveDirectoryGroups(config.ActiveDirectoryGroupCount, [.. this._context.NonHumanIdentities]);
        await this.SeedActiveDirectoryGroupMemberships(config.ActiveDirectoryGroupMembershipCount, [.. this._context.ActiveDirectoryGroups], [.. this._context.ActiveDirectoryAccounts]);
        await this.SeedBusinessApplicationResources(config.BusinessApplicationResourceCount, businessApplications, sqlServers, databases);
        await this.SeedServerLogins(config.ServerLoginCount, sqlServers, [.. this._context.ActiveDirectoryAccounts], [.. this._context.ActiveDirectoryGroups], [.. this._context.NonHumanIdentities]);
        await this.SeedServerRoles(config.ServerRoleCount, sqlServers);
        await this.SeedDatabaseUsers(config.DatabaseUserCount, databases, [.. this._context.ServerLogins]);
        await SeedDatabaseRoles(config.DatabaseRoleCount, databases);
        await this.SeedPermissions(config.PermissionCount, databases, [.. this._context.DatabaseUsers], [.. this._context.DatabaseRoles], [.. this._context.ServerRoles]);
        await this.SeedServerLoginRoles(config.ServerLoginRoleCount, [.. this._context.ServerLogins], [.. this._context.ServerRoles]);
        await this.SeedDatabaseUserRoles(config.DatabaseUserRoleCount, [.. this._context.DatabaseUsers], [.. this._context.DatabaseRoles]);
        await this.SeedAdminAuditLogs(config.AdminAuditLogCount);

        Console.WriteLine("Data seeding completed.");
    }

    // --- Configuration Class for Seeding ---
    public class SeedConfiguration
    {
        public int HumanIdentityCount { get; set; } = 100;
        public int BusinessApplicationCount { get; set; } = 20;
        public int BusinessApplicationEnvironmentCount { get; set; } = 50;
        public int BusinessApplicationTeamMemberCount { get; set; } = 150;
        public int NonHumanIdentityCount { get; set; } = 100;
        public int ActiveDirectoryAccountCount { get; set; } = 150;
        public int ActiveDirectoryGroupCount { get; set; } = 50;
        public int ActiveDirectoryGroupMembershipCount { get; set; } = 300;
        public int BusinessApplicationResourceCount { get; set; } = 100;
        public int SqlServerCount { get; set; } = 10;
        public int SqlServerInstanceCount { get; set; } = 20; // This will be calculated based on SqlServerCount
        public int DatabaseCount { get; set; } = 50; // This will be calculated based on SqlServerInstanceCount
        public int ServerLoginCount { get; set; } = 150;
        public int ServerRoleCount { get; set; } = 30;
        public int DatabaseUserCount { get; set; } = 200;
        public int DatabaseRoleCount { get; set; } = 80;
        public int PermissionCount { get; set; } = 500;
        public int ServerLoginRoleCount { get; set; } = 250;
        public int DatabaseUserRoleCount { get; set; } = 300;
        public int AdminAuditLogCount { get; set; } = 100;
    }

    // --- Seeding Methods for Each Entity ---

    private async Task<List<HumanIdentity>> SeedHumanIdentities(int count)
    {
        if (await this._context.HumanIdentities.AnyAsync())
            return [];

        var faker = new Faker<HumanIdentity>()
            .RuleFor(hi => hi.Id, f => 0) // Id will be set by EF Core
            .RuleFor(hi => hi.FirstName, f => f.Name.FirstName())
            .RuleFor(hi => hi.LastName, f => f.Name.LastName())
            .RuleFor(hi => hi.UserId, (f, hi) => $"{hi.FirstName.ToLowerInvariant()}.{hi.LastName.ToLowerInvariant()}{f.Random.Int(1, 100):00}") // meaningful userId
            .RuleFor(hi => hi.Email, (f, hi) => f.Internet.Email(hi.FirstName, hi.LastName).ToLowerInvariant()) // meaningful email
            .RuleFor(hi => hi.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(hi => hi.JobTitle, f => f.Name.JobTitle())
            .RuleFor(hi => hi.Department, f => f.Commerce.Department())
            .RuleFor(hi => hi.Division, f => f.Company.CompanyName()) // Changed from BsAdjective
            .RuleFor(hi => hi.CostCenter, f => f.Finance.Account(6))
            .RuleFor(hi => hi.Location, f => f.Address.City())
            .RuleFor(hi => hi.Manager, f => f.Name.FullName())
            .RuleFor(hi => hi.HireDate, f => ToUtc(f.Date.Past(10)))
            .RuleFor(hi => hi.TerminationDate, (f, hi) => ToUtc(f.Date.Between(hi.HireDate.GetValueOrDefault(), DateTime.UtcNow).OrNull(f, 0.2f)))
            .RuleFor(hi => hi.Status, f => f.PickRandom<HumanIdentityStatus>())
            .RuleFor(hi => hi.EmployeeId, f => f.Finance.Account(8))
            .RuleFor(hi => hi.Company, f => f.Company.CompanyName())
            .RuleFor(hi => hi.IsContractor, f => f.Random.Bool(0.2f))
            .RuleFor(hi => hi.ContractEndDate, (f, hi) => hi.IsContractor ? ToUtc(f.Date.Future(2)) : null)
            .RuleFor(hi => hi.Description, f => f.Lorem.Sentence())
            .RuleFor(hi => hi.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(hi => hi.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this._context.HumanIdentities.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Human Identities.");
        return entities;
    }

    private async Task<List<BusinessApplication>> SeedBusinessApplications(int count, List<HumanIdentity> humanIdentities)
    {
        if (await this._context.BusinessApplications.AnyAsync() || !humanIdentities.Any())
            return [];

        var faker = new Faker<BusinessApplication>()
            .RuleFor(ba => ba.Id, f => 0)
            .RuleFor(ba => ba.Name, f => $"{f.Commerce.ProductName()} {f.Hacker.Noun()}")
            .RuleFor(ba => ba.ShortName, (f, ba) => string.Join("", ba.Name.Split(' ').Select(s => s[0]).Take(3)).ToUpperInvariant() + f.Random.Int(100, 999))
            .RuleFor(ba => ba.Description, f => f.Lorem.Sentence(5, 10))
            .RuleFor(ba => ba.BusinessPurpose, f => f.Lorem.Paragraph())
            .RuleFor(ba => ba.Status, f => f.PickRandom<BusinessApplicationStatus>())
            .RuleFor(ba => ba.Criticality, f => f.PickRandom<BusinessApplicationCriticality>())
            .RuleFor(ba => ba.ApplicationOwnerId, f => f.PickRandom(humanIdentities).Id)
            .RuleFor(ba => ba.AlternateApplicationOwnerId, (f, ba) => f.PickRandom(humanIdentities.Where(hi => hi.Id != ba.ApplicationOwnerId)).Id.OrNull(f, 0.3f))
            .RuleFor(ba => ba.ProductOwnerId, (f, ba) => f.PickRandom(humanIdentities).Id)
            .RuleFor(ba => ba.AlternateProductOwnerId, (f, ba) => f.PickRandom(humanIdentities.Where(hi => hi.Id != ba.ProductOwnerId)).Id.OrNull(f, 0.3f))
            .RuleFor(ba => ba.TechnicalContact, f => f.Internet.Email())
            .RuleFor(ba => ba.BusinessContact, f => f.Internet.Email())
            .RuleFor(ba => ba.VendorName, f => f.Company.CompanyName())
            .RuleFor(ba => ba.Version, f => f.System.Semver())
            .RuleFor(ba => ba.GoLiveDate, f => ToUtc(f.Date.Past(5)))
            .RuleFor(ba => ba.EndOfLifeDate, (f, ba) => ToUtc(f.Date.Future(5).OrNull(f, 0.1f)))
            .RuleFor(ba => ba.AnnualCost, f => f.Finance.Amount(1000, 100000))
            .RuleFor(ba => ba.ComplianceRequirements, f => f.Lorem.Sentence())
            .RuleFor(ba => ba.DataClassification, f => f.PickRandom(new[] { "Public", "Internal", "Confidential", "Highly Confidential" }))
            .RuleFor(ba => ba.IsCustomDeveloped, f => f.Random.Bool())
            .RuleFor(ba => ba.SourceCodeRepository, (f, ba) => ba.IsCustomDeveloped ? f.Internet.UrlWithPath("github.com") : null)
            .RuleFor(ba => ba.DocumentationLink, f => f.Internet.Url())
            .RuleFor(ba => ba.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ba => ba.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this._context.BusinessApplications.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Applications.");
        return entities;
    }

    private async Task<List<SqlServer>> SeedSqlServers(int count)
    {
        if (await this._context.SqlServers.AnyAsync())
            return [];

        var faker = new Faker<SqlServer>()
            .RuleFor(ss => ss.Id, f => 0)
            .RuleFor(ss => ss.Name, f => $"SQL-{f.Address.City().Replace(" ", "")}-{f.Random.Int(100, 999)}")
            .RuleFor(ss => ss.HostName, (f, ss) => $"{ss.Name.ToLowerInvariant()}.example.com")
            .RuleFor(ss => ss.IPAddress, f => f.Internet.Ip())
            .RuleFor(ss => ss.Version, f => f.PickRandom(new[] { "SQL Server 2019", "SQL Server 2022", "SQL Server 2017" }))
            .RuleFor(ss => ss.Edition, f => f.PickRandom(new[] { "Enterprise", "Standard", "Developer" }))
            .RuleFor(ss => ss.ServicePack, f => f.PickRandom(new[] { "SP1", "SP2", "CU1", "RTM" }))
            .RuleFor(ss => ss.IsActive, f => f.Random.Bool())
            .RuleFor(ss => ss.Description, f => f.Lorem.Sentence())
            .RuleFor(ss => ss.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ss => ss.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this._context.SqlServers.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} SQL Servers.");
        return entities;
    }

    private async Task<List<SqlServerInstance>> SeedSqlServerInstances(List<SqlServer> sqlServers)
    {
        if (await this._context.SqlServerInstances.AnyAsync() || !sqlServers.Any())
            return [];

        var possibleNames = new[] { "MSSQLSERVER", "PROD", "DEV", "UAT" };
        var entities = new List<SqlServerInstance>();
        var faker = new Faker<SqlServerInstance>()
            .RuleFor(ssi => ssi.Id, f => 0)
            .RuleFor(ssi => ssi.Port, f => f.PickRandom(new[] { "1433", "1434", "50000", "50001" }))
            .RuleFor(ssi => ssi.ServiceAccount, f => $"domain\\{f.Internet.UserName()}")
            .RuleFor(ssi => ssi.Collation, f => f.PickRandom(new[] { "SQL_Latin1_General_CP1_CI_AS", "Latin1_General_CI_AS" }))
            .RuleFor(ssi => ssi.IsActive, f => f.Random.Bool())
            .RuleFor(ssi => ssi.Description, f => f.Lorem.Sentence())
            .RuleFor(ssi => ssi.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ssi => ssi.CreatedBy, f => f.Internet.UserName());

        var randomFaker = new Faker(); // For randomization

        foreach (var server in sqlServers)
        {
            // Generate 2 unique names per server
            var usedNames = new HashSet<string>();
            int instancesPerServer = 2;
            for (int i = 0; i < instancesPerServer; i++)
            {
                string baseName = randomFaker.PickRandom(possibleNames);
                string name = baseName;
                int suffix = 1;
                while (usedNames.Contains(name))
                {
                    name = $"{baseName}_{randomFaker.Random.AlphaNumeric(4).ToUpperInvariant()}_{suffix}";
                    suffix++;
                }
                usedNames.Add(name);
                var instance = faker.Generate();
                instance.ServerId = server.Id;
                instance.Name = name;
                instance.ServiceName = $"MSSQL${name}";
                entities.Add(instance);
            }
        }
        await this._context.SqlServerInstances.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} SQL Server Instances.");
        return entities;
    }

    private async Task<List<IAMBuddy.Tools.Data.Entities.Database>> SeedDatabases(List<SqlServerInstance> sqlServerInstances)
    {
        if (await this._context.Databases.AnyAsync() || !sqlServerInstances.Any())
            return [];

        var faker = new Faker<IAMBuddy.Tools.Data.Entities.Database>()
            .RuleFor(db => db.Id, f => 0)
            .RuleFor(db => db.InstanceId, f => f.PickRandom(sqlServerInstances).Id)
            .RuleFor(db => db.Name, f => f.Database.Engine().Replace(" ", "") + f.Random.Int(100, 999))
            .RuleFor(db => db.Collation, f => f.PickRandom(new[] { "SQL_Latin1_General_CP1_CI_AS", "Latin1_General_CI_AS" }))
            .RuleFor(db => db.RecoveryModel, f => f.PickRandom(new[] { "FULL", "SIMPLE", "BULK_LOGGED" }))
            .RuleFor(db => db.CompatibilityLevel, f => f.PickRandom(new[] { "150", "160", "140" }))
            .RuleFor(db => db.IsActive, f => f.Random.Bool())
            .RuleFor(db => db.Description, f => f.Lorem.Sentence())
            .RuleFor(db => db.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(db => db.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(sqlServerInstances.Count * 3); // 3 databases per instance on average
        await this._context.Databases.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Databases.");
        return entities;
    }

    private async Task SeedBusinessApplicationEnvironments(int count, List<BusinessApplication> businessApplications)
    {
        if (await this._context.BusinessApplicationEnvironments.AnyAsync() || !businessApplications.Any())
            return;

        var faker = new Faker<BusinessApplicationEnvironment>()
            .RuleFor(bae => bae.Id, f => 0)
            .RuleFor(bae => bae.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(bae => bae.Environment, f => f.PickRandom<ApplicationEnvironment>())
            .RuleFor(bae => bae.EnvironmentName, (f, bae) => bae.Environment.ToString())
            .RuleFor(bae => bae.Description, f => f.Lorem.Sentence())
            .RuleFor(bae => bae.IsActive, f => f.Random.Bool())
            .RuleFor(bae => bae.Url, (f, bae) => 
            {
                var businessApp = businessApplications.FirstOrDefault(ba => ba.Id == bae.BusinessApplicationId);
                return f.Internet.UrlWithPath(businessApp?.ShortName?.ToLowerInvariant() ?? "app");
            })
            .RuleFor(bae => bae.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(bae => bae.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this._context.BusinessApplicationEnvironments.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Application Environments.");
    }

    private async Task SeedBusinessApplicationTeamMembers(int count, List<BusinessApplication> businessApplications, List<HumanIdentity> humanIdentities)
    {
        if (await this._context.BusinessApplicationTeamMembers.AnyAsync() || !businessApplications.Any() || !humanIdentities.Any())
            return;

        var faker = new Faker<BusinessApplicationTeamMember>()
            .RuleFor(batm => batm.Id, f => 0)
            .RuleFor(batm => batm.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(batm => batm.HumanIdentityId, f => f.PickRandom(humanIdentities).Id)
            .RuleFor(batm => batm.Role, f => f.PickRandom(new[] { "Developer", "QA Engineer", "Product Manager", "Scrum Master", "Business Analyst" }))
            .RuleFor(batm => batm.IsPrimary, f => f.Random.Bool())
            .RuleFor(batm => batm.StartDate, f => ToUtc(f.Date.Past(2)))
            .RuleFor(batm => batm.EndDate, (f, batm) => ToUtc(f.Date.Future(1).OrNull(f, 0.4f)))
            .RuleFor(batm => batm.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(batm => batm.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this._context.BusinessApplicationTeamMembers.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Application Team Members.");
    }

    private async Task SeedNonHumanIdentities(int count, List<BusinessApplication> businessApplications, List<HumanIdentity> humanIdentities, List<BusinessApplicationEnvironment> environments)
    {
        if (await this._context.NonHumanIdentities.AnyAsync() || !businessApplications.Any() || !humanIdentities.Any())
            return;

        var faker = new Faker<NonHumanIdentity>()
            .RuleFor(nhi => nhi.Id, f => 0)
            .RuleFor(nhi => nhi.Name, (f, nhi) => $"{f.System.CommonFileName(f.Random.Word())}-{f.Random.Int(100, 999)}")
            .RuleFor(nhi => nhi.DisplayName, (f, nhi) => nhi.Name.Replace("-", " "))
            .RuleFor(nhi => nhi.Type, f => f.PickRandom<NonHumanIdentityType>())
            .RuleFor(nhi => nhi.Status, f => f.PickRandom<NonHumanIdentityStatus>())
            .RuleFor(nhi => nhi.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(nhi => nhi.BusinessApplicationEnvironmentId, (f, nhi) => {
                var envs = environments.Where(e => e.BusinessApplicationId == nhi.BusinessApplicationId).ToList();
                if (envs.Count == 0) return null;
                return f.PickRandom(envs).Id.OrNull(f, 0.3f);
            })
            .RuleFor(nhi => nhi.PrimaryOwnerId, f => f.PickRandom(humanIdentities).Id)
            .RuleFor(nhi => nhi.AlternateOwnerId, (f, nhi) => f.PickRandom(humanIdentities.Where(hi => hi.Id != nhi.PrimaryOwnerId)).Id.OrNull(f, 0.3f))
            .RuleFor(nhi => nhi.Purpose, f => f.Lorem.Sentence())
            .RuleFor(nhi => nhi.TechnicalContact, f => f.Internet.Email())
            .RuleFor(nhi => nhi.ExpirationDate, f => ToUtc(f.Date.Future(3).OrNull(f, 0.2f)))
            .RuleFor(nhi => nhi.LastAccessDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(nhi => nhi.AccessFrequency, f => f.PickRandom(new[] { "Daily", "Weekly", "Monthly", "Rarely" }))
            .RuleFor(nhi => nhi.IsGeneric, f => f.Random.Bool(0.1f))
            .RuleFor(nhi => nhi.Description, f => f.Lorem.Sentence())
            .RuleFor(nhi => nhi.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(nhi => nhi.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this._context.NonHumanIdentities.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Non-Human Identities.");
    }

    private async Task SeedActiveDirectoryAccounts(int count, List<HumanIdentity> humanIdentities, List<NonHumanIdentity> nonHumanIdentities)
    {
        if (await this._context.ActiveDirectoryAccounts.AnyAsync() || (!humanIdentities.Any() && !nonHumanIdentities.Any()))
            return;

        var faker = new Faker<ActiveDirectoryAccount>()
            .RuleFor(ada => ada.Id, f => 0)
            .RuleFor(ada => ada.AccountType, f => f.PickRandom<ActiveDirectoryAccountType>())
            .RuleFor(ada => ada.Domain, f => f.PickRandom(new[] { "corp.example.com", "dev.example.com", "contoso.local" }))
            .RuleFor(ada => ada.IsEnabled, f => f.Random.Bool())
            .RuleFor(ada => ada.LastLogonDate, f => ToUtc(f.Date.Recent().OrNull(f, 0.1f)))
            .RuleFor(ada => ada.PasswordLastSetDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ada => ada.AccountExpirationDate, (f, ada) => ada.IsEnabled ? ToUtc(f.Date.Future(2).OrNull(f, 0.2f)) : null)
            .RuleFor(ada => ada.PasswordNeverExpires, f => f.Random.Bool(0.1f))
            .RuleFor(ada => ada.UserCannotChangePassword, f => f.Random.Bool(0.1f))
            .RuleFor(ada => ada.ServicePrincipalNames, (f, ada) => ada.AccountType == ActiveDirectoryAccountType.ServiceAccount ? System.Text.Json.JsonSerializer.Serialize(f.Make(f.Random.Int(1, 3), () => f.Internet.DomainName())) : null)
            .RuleFor(ada => ada.ManagedBy, f => f.Internet.Email())
            .RuleFor(ada => ada.Description, f => f.Lorem.Sentence())
            .RuleFor(ada => ada.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ada => ada.CreatedBy, f => f.Internet.UserName())
            .RuleFor(ada => ada.HumanIdentityId, (f, ada) =>
            {
                if (ada.AccountType == ActiveDirectoryAccountType.User && humanIdentities.Any())
                {
                    return f.PickRandom(humanIdentities).Id;
                }
                return (int?)null;
            })
            .RuleFor(ada => ada.NonHumanIdentityId, (f, ada) =>
            {
                if (ada.AccountType == ActiveDirectoryAccountType.ServiceAccount && nonHumanIdentities.Any())
                {
                    return f.PickRandom(nonHumanIdentities).Id;
                }
                return (int?)null;
            });

        var entities = new List<ActiveDirectoryAccount>();
        var samAccountNameCache = new HashSet<string>();
        var upnCache = new HashSet<string>();
        var distinguishedNameCache = new HashSet<string>();
        var randomizer = new Randomizer();

        int attempts = 0;
        int maxAttempts = count * 2; // Set a reasonable limit to prevent infinite loops
        
        while (entities.Count < count && attempts < maxAttempts)
        {
            var entity = faker.Generate();
            
            // Generate SamAccountName based on identity
            if (entity.HumanIdentityId.HasValue)
            {
                var human = humanIdentities.FirstOrDefault(hi => hi.Id == entity.HumanIdentityId.Value);
                var baseAccountName = $"{human?.FirstName.ToLowerInvariant() ?? randomizer.AlphaNumeric(5)}{human?.LastName.ToLowerInvariant().Substring(0, 1) ?? randomizer.AlphaNumeric(1)}";
                var samAccountName = baseAccountName;
                int suffix = 1;
                while (samAccountNameCache.Contains(samAccountName))
                {
                    samAccountName = $"{baseAccountName}{suffix:000}";
                    suffix++;
                }
                entity.SamAccountName = samAccountName;
            }
            else if (entity.NonHumanIdentityId.HasValue)
            {
                var nonHuman = nonHumanIdentities.FirstOrDefault(nhi => nhi.Id == entity.NonHumanIdentityId.Value);
                var baseAccountName = $"svc_{nonHuman?.Name.ToLowerInvariant().Replace(" ", "") ?? randomizer.AlphaNumeric(8)}";
                var samAccountName = baseAccountName;
                int suffix = 1;
                while (samAccountNameCache.Contains(samAccountName))
                {
                    samAccountName = $"{baseAccountName}_{suffix:000}";
                    suffix++;
                }
                entity.SamAccountName = samAccountName;
            }
            else
            {
                var baseAccountName = $"generic_{randomizer.AlphaNumeric(8)}";
                var samAccountName = baseAccountName;
                int suffix = 1;
                while (samAccountNameCache.Contains(samAccountName))
                {
                    samAccountName = $"{baseAccountName}_{suffix:000}";
                    suffix++;
                }
                entity.SamAccountName = samAccountName;
            }

            // Generate UPN based on SamAccountName
            var upn = $"{entity.SamAccountName}@{entity.Domain}";
            // Set display name based on the account type
            if (entity.HumanIdentityId.HasValue)
            {
                var human = humanIdentities.FirstOrDefault(hi => hi.Id == entity.HumanIdentityId.Value);
                entity.DisplayName = $"{human?.FirstName} {human?.LastName}";
            }
            else if (entity.NonHumanIdentityId.HasValue)
            {
                var nonHuman = nonHumanIdentities.FirstOrDefault(nhi => nhi.Id == entity.NonHumanIdentityId.Value);
                entity.DisplayName = nonHuman?.DisplayName ?? entity.SamAccountName;
            }
            else
            {
                entity.DisplayName = entity.SamAccountName;
            }

            // Generate unique distinguished name
            var baseDn = $"CN={entity.DisplayName},OU=Users,DC={entity.Domain.Replace(".", ",DC=")}";
            var dn = baseDn;
            int dnSuffix = 1;
            while (distinguishedNameCache.Contains(dn))
            {
                dn = $"CN={entity.DisplayName}_{dnSuffix},OU=Users,DC={entity.Domain.Replace(".", ",DC=")}";
                dnSuffix++;
            }
            entity.DistinguishedName = dn;

            // Only add if all unique constraints are satisfied
            if (!samAccountNameCache.Contains(entity.SamAccountName) && !upnCache.Contains(upn) && !distinguishedNameCache.Contains(dn))
            {
                entity.UserPrincipalName = upn;
                samAccountNameCache.Add(entity.SamAccountName);
                upnCache.Add(upn);
                distinguishedNameCache.Add(dn);
                entities.Add(entity);
            }
            attempts++;
        }

        await this._context.ActiveDirectoryAccounts.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Active Directory Accounts.");
    }

    private async Task SeedActiveDirectoryGroups(int count, List<NonHumanIdentity> nonHumanIdentities)
    {
        if (await this._context.ActiveDirectoryGroups.AnyAsync())
            return;

        var faker = new Faker<ActiveDirectoryGroup>()
            .RuleFor(adg => adg.Id, f => 0)
            .RuleFor(adg => adg.GroupType, f => f.PickRandom<ActiveDirectoryGroupType>())
            .RuleFor(adg => adg.GroupScope, f => f.PickRandom<ActiveDirectoryGroupScope>())
            .RuleFor(adg => adg.Domain, f => f.PickRandom(new[] { "corp.example.com", "dev.example.com", "contoso.local" }))
            .RuleFor(adg => adg.IsActive, f => f.Random.Bool())
            .RuleFor(adg => adg.Email, (f, adg) => f.Internet.Email(adg.Name.Replace(" ", ".")))
            .RuleFor(adg => adg.ManagedBy, (f, adg) => f.Random.Bool(0.7f) && nonHumanIdentities.Any() ? f.PickRandom(nonHumanIdentities).Name : f.Internet.UserName())
            .RuleFor(adg => adg.NonHumanIdentityId, (f, adg) => f.Random.Bool(0.7f) && nonHumanIdentities.Any() ? f.PickRandom(nonHumanIdentities).Id : (int?)null)
            .RuleFor(adg => adg.Description, f => f.Lorem.Sentence())
            .RuleFor(adg => adg.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(adg => adg.CreatedBy, f => f.Internet.UserName())
            .RuleFor(adg => adg.Name, (f, adg) =>
            {
                var typePrefix = adg.GroupType == ActiveDirectoryGroupType.Security ? "SG" : "DL";
                var scopeSuffix = adg.GroupScope switch
                {
                    ActiveDirectoryGroupScope.DomainLocal => "DL",
                    ActiveDirectoryGroupScope.Global => "G",
                    ActiveDirectoryGroupScope.Universal => "U",
                    _ => ""
                };
                return $"{typePrefix}_{f.Commerce.Department()}_{scopeSuffix}_{f.Random.Int(100, 999)}".Replace(" ", "_");
            })
            .RuleFor(adg => adg.SamAccountName, (f, adg) => adg.Name)
            .RuleFor(adg => adg.DisplayName, (f, adg) => adg.Name.Replace("_", " "));

        var entities = faker.Generate(count);
        await this._context.ActiveDirectoryGroups.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Active Directory Groups.");
    }

    private async Task SeedActiveDirectoryGroupMemberships(int count, List<ActiveDirectoryGroup> groups, List<ActiveDirectoryAccount> accounts)
    {
        if (await this._context.ActiveDirectoryGroupMemberships.AnyAsync() || !groups.Any())
            return;

        var faker = new Faker<ActiveDirectoryGroupMembership>()
            .RuleFor(adgm => adgm.Id, f => 0)
            .RuleFor(adgm => adgm.GroupId, f => f.PickRandom(groups).Id)
            .RuleFor(adgm => adgm.AccountId, (f, adgm) => f.Random.Bool(0.7f) && accounts.Any() ? f.PickRandom(accounts).Id : (int?)null)
            .RuleFor(adgm => adgm.ChildGroupId, (f, adgm) => f.Random.Bool(0.3f) && groups.Any() ? f.PickRandom(groups.Where(g => g.Id != adgm.GroupId)).Id : (int?)null) // Ensure no self-referencing
            .RuleFor(adgm => adgm.MemberSince, f => f.Date.Past(1))
            .RuleFor(adgm => adgm.AddedBy, f => f.Internet.UserName())
            .RuleFor(adgm => adgm.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(adgm => adgm.CreatedBy, f => f.Internet.UserName());

        // Ensure at least one of AccountId or ChildGroupId is set
        var entities = new List<ActiveDirectoryGroupMembership>();
        var generatedCount = 0;
        while (generatedCount < count)
        {
            var membership = faker.Generate();
            if (membership.AccountId.HasValue || membership.ChildGroupId.HasValue)
            {
                // Prevent duplicate memberships based on GroupId, AccountId, ChildGroupId combination
                if (!entities.Any(e => e.GroupId == membership.GroupId &&
                                       e.AccountId == membership.AccountId &&
                                       e.ChildGroupId == membership.ChildGroupId))
                {
                    entities.Add(membership);
                    generatedCount++;
                }
            }
        }

        await this._context.ActiveDirectoryGroupMemberships.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Active Directory Group Memberships.");
    }

    private async Task SeedBusinessApplicationResources(int count, List<BusinessApplication> businessApplications, List<SqlServer> sqlServers, List<IAMBuddy.Tools.Data.Entities.Database> databases)
    {
        if (await this._context.BusinessApplicationResources.AnyAsync() || !businessApplications.Any())
            return;

        var faker = new Faker<BusinessApplicationResource>()
            .RuleFor(bar => bar.Id, f => 0)
            .RuleFor(bar => bar.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(bar => bar.ResourceType, f => f.PickRandom(new[] { "Server", "Database" })) // Extend as needed
            .RuleFor(bar => bar.Environment, f => f.PickRandom<ApplicationEnvironment>())
            .RuleFor(bar => bar.Purpose, f => f.Lorem.Sentence())
            .RuleFor(bar => bar.IsCritical, f => f.Random.Bool(0.3f))
            .RuleFor(bar => bar.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(bar => bar.CreatedBy, f => f.Internet.UserName())
            .RuleFor(bar => bar.ResourceId, (f, bar) =>
            {
                if (bar.ResourceType == "Server" && sqlServers.Any())
                {
                    return f.PickRandom(sqlServers).Id;
                }
                else if (bar.ResourceType == "Database" && databases.Any())
                {
                    return f.PickRandom(databases).Id;
                }
                return 0; // Will be filtered out or throw if 0 is not valid
            })
            .RuleFor(bar => bar.ResourceName, (f, bar) =>
            {
                if (bar.ResourceType == "Server" && sqlServers.Any())
                {
                    return sqlServers.FirstOrDefault(s => s.Id == bar.ResourceId)?.Name;
                }
                else if (bar.ResourceType == "Database" && databases.Any())
                {
                    return databases.FirstOrDefault(d => d.Id == bar.ResourceId)?.Name;
                }
                return null;
            });

        var entities = faker.Generate(count).Where(e => e.ResourceId != 0).ToList(); // Filter out entities where ResourceId wasn't set correctly
        await this._context.BusinessApplicationResources.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Application Resources.");
    }

    private async Task SeedServerLogins(int count, List<SqlServer> sqlServers, List<ActiveDirectoryAccount> adAccounts, List<ActiveDirectoryGroup> adGroups, List<NonHumanIdentity> nonHumanIdentities)
    {
        if (await this._context.ServerLogins.AnyAsync() || !sqlServers.Any())
            return;

        var faker = new Faker<ServerLogin>()
            .RuleFor(sl => sl.Id, f => 0)
            .RuleFor(sl => sl.ServerId, f => f.PickRandom(sqlServers).Id)
            .RuleFor(sl => sl.AuthenticationType, f => f.PickRandom<AuthenticationType>())
            .RuleFor(sl => sl.Status, f => f.PickRandom<LoginStatus>())
            .RuleFor(sl => sl.DefaultDatabase, f => f.Database.Engine().OrNull(f, 0.2f))
            .RuleFor(sl => sl.DefaultLanguage, f => f.PickRandom(new[] { "us_english", "Japanese", "French" }))
            .RuleFor(sl => sl.PasswordExpirationDate, f => ToUtc(f.Date.Future(1).OrNull(f, 0.3f)))
            .RuleFor(sl => sl.IsPasswordExpired, (f, sl) => sl.PasswordExpirationDate.HasValue && sl.PasswordExpirationDate < DateTime.Now)
            .RuleFor(sl => sl.IsLocked, (f, sl) => sl.Status == LoginStatus.Locked)
            .RuleFor(sl => sl.LastLoginDate, f => ToUtc(f.Date.Recent()))
            .RuleFor(sl => sl.Description, f => f.Lorem.Sentence())
            .RuleFor(sl => sl.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(sl => sl.CreatedBy, f => f.Internet.UserName())
            .RuleFor(sl => sl.ActiveDirectoryAccountId, (f, sl) =>
            {
                if (sl.AuthenticationType == AuthenticationType.AzureAD || sl.AuthenticationType == AuthenticationType.Windows)
                {
                    if (f.Random.Bool(0.7f) && adAccounts.Any())
                        return f.PickRandom(adAccounts).Id;
                }
                return (int?)null;
            })
            .RuleFor(sl => sl.ActiveDirectoryGroupId, (f, sl) =>
            {
                if ((sl.AuthenticationType == AuthenticationType.AzureAD || sl.AuthenticationType == AuthenticationType.Windows) && !sl.ActiveDirectoryAccountId.HasValue)
                {
                    if (f.Random.Bool(0.5f) && adGroups.Any())
                        return f.PickRandom(adGroups).Id;
                }
                return (int?)null;
            })
            .RuleFor(sl => sl.NonHumanIdentityId, (f, sl) =>
            {
                if (sl.AuthenticationType == AuthenticationType.SqlServer || sl.AuthenticationType == AuthenticationType.Windows) // SQL Server or Windows service accounts
                {
                    if (f.Random.Bool(0.2f) && nonHumanIdentities.Any())
                        return f.PickRandom(nonHumanIdentities).Id;
                }
                return (int?)null;
            })
            .RuleFor(sl => sl.Name, (f, sl) =>
            {
                if (sl.ActiveDirectoryAccountId.HasValue)
                {
                    return adAccounts.FirstOrDefault(ada => ada.Id == sl.ActiveDirectoryAccountId)?.SamAccountName;
                }
                else if (sl.ActiveDirectoryGroupId.HasValue)
                {
                    return adGroups.FirstOrDefault(adg => adg.Id == sl.ActiveDirectoryGroupId)?.SamAccountName;
                }
                else if (sl.NonHumanIdentityId.HasValue)
                {
                    return nonHumanIdentities.FirstOrDefault(nhi => nhi.Id == sl.NonHumanIdentityId)?.Name;
                }
                return f.Internet.UserName(); // For SQL Server auth
            });

        var entities = faker.Generate(count);
        await this._context.ServerLogins.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Server Logins.");
    }

    private async Task SeedServerRoles(int count, List<SqlServer> sqlServers)
    {
        if (await this._context.ServerRoles.AnyAsync() || !sqlServers.Any())
            return;

        var faker = new Faker<ServerRole>()
            .RuleFor(sr => sr.Id, f => 0)
            .RuleFor(sr => sr.ServerId, f => f.PickRandom(sqlServers).Id)
            .RuleFor(sr => sr.Type, f => RoleType.ServerRole)
            .RuleFor(sr => sr.IsFixedRole, f => f.Random.Bool(0.5f))
            .RuleFor(sr => sr.Description, f => f.Lorem.Sentence())
            .RuleFor(sr => sr.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(sr => sr.CreatedBy, f => f.Internet.UserName())
            .RuleFor(sr => sr.Name, (f, sr) =>
            {
                if (sr.IsFixedRole)
                {
                    return f.PickRandom(new[] { "sysadmin", "serveradmin", "securityadmin", "setupadmin", "processadmin", "diskadmin", "dbcreator", "bulkadmin", "public" });
                }
                return $"CustomServerRole_{f.Commerce.Department().Replace(" ", "")}";
            });

        var entities = faker.Generate(count);
        await this._context.ServerRoles.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Server Roles.");
    }

    private async Task SeedDatabaseUsers(int count, List<IAMBuddy.Tools.Data.Entities.Database> databases, List<ServerLogin> serverLogins)
    {
        if (await this._context.DatabaseUsers.AnyAsync() || !databases.Any())
            return;

        var faker = new Faker<DatabaseUser>()
            .RuleFor(du => du.Id, f => 0)
            .RuleFor(du => du.DatabaseId, f => f.PickRandom(databases).Id)
            .RuleFor(du => du.UserType, f => f.PickRandom(new[] { "SQL_USER", "WINDOWS_USER", "WINDOWS_GROUP", "AZUREAD_USER", "AZUREAD_GROUP" }))
            .RuleFor(du => du.ServerLoginId, (f, du) => 
            {
                // Get valid server logins based on user type
                var validLogins = du.UserType switch
                {
                    "SQL_USER" => serverLogins.Where(sl => sl.AuthenticationType == AuthenticationType.SqlServer),
                    "WINDOWS_USER" => serverLogins.Where(sl => sl.AuthenticationType == AuthenticationType.Windows && sl.ActiveDirectoryAccountId.HasValue),
                    "WINDOWS_GROUP" => serverLogins.Where(sl => sl.AuthenticationType == AuthenticationType.Windows && sl.ActiveDirectoryGroupId.HasValue),
                    "AZUREAD_USER" => serverLogins.Where(sl => sl.AuthenticationType == AuthenticationType.AzureAD && sl.ActiveDirectoryAccountId.HasValue),
                    "AZUREAD_GROUP" => serverLogins.Where(sl => sl.AuthenticationType == AuthenticationType.AzureAD && sl.ActiveDirectoryGroupId.HasValue),
                    _ => Enumerable.Empty<ServerLogin>()
                };

                return validLogins.Any() ? f.PickRandom(validLogins).Id : null;
            })
            .RuleFor(du => du.DefaultSchema, (f, du) => du.UserType switch
            {
                "SQL_USER" => "dbo", // SQL users often use dbo schema
                "WINDOWS_USER" or "AZUREAD_USER" => f.PickRandom(new[] { "dbo", "app", "data" }), // Individual users might have specific schemas
                "WINDOWS_GROUP" or "AZUREAD_GROUP" => f.PickRandom(new[] { "readonly", "app_user", "app_admin" }), // Groups often have role-based schemas
                _ => "public" // Fallback schema
            })
            .RuleFor(du => du.IsActive, f => f.Random.Bool(0.9f)) // 90% chance of being active
            .RuleFor(du => du.Description, (f, du) => 
            {
                var loginInfo = du.ServerLoginId.HasValue ? 
                    serverLogins.FirstOrDefault(sl => sl.Id == du.ServerLoginId)?.Name : "No associated login";
                return $"{du.UserType} database user for {loginInfo}";
            })
            .RuleFor(du => du.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(du => du.CreatedBy, f => f.Internet.UserName())
            .RuleFor(du => du.Name, (f, du) =>
            {
                if (du.ServerLoginId.HasValue)
                {
                    var login = serverLogins.FirstOrDefault(sl => sl.Id == du.ServerLoginId);
                    if (login != null)
                    {
                        // For Windows/Azure AD logins, remove domain prefix if present
                        var name = login.Name;
                        if (name.Contains('\\'))
                        {
                            name = name[(name.IndexOf('\\') + 1)..];
                        }
                        return name;
                    }
                }
                // Generate a meaningful name for users without logins
                return $"{du.UserType.ToLower()}_{f.Internet.UserName()}";
            });

        var entities = faker.Generate(count)
            // Ensure we have unique names per database
            .GroupBy(du => new { du.DatabaseId, du.Name })
            .Select(g => g.First())
            .ToList();

        await this._context.DatabaseUsers.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Database Users.");
    }

    private async Task SeedDatabaseRoles(int count, List<IAMBuddy.Tools.Data.Entities.Database> databases)
    {
        if (await this._context.DatabaseRoles.AnyAsync() || !databases.Any())
            return;

        var faker = new Faker<DatabaseRole>()
            .RuleFor(dr => dr.Id, f => 0)
            .RuleFor(dr => dr.DatabaseId, (f, dr) => f.PickRandom(databases).Id)
            .RuleFor(dr => dr.Type, f => RoleType.DatabaseRole)
            .RuleFor(dr => dr.IsFixedRole, f => f.Random.Bool(0.5f))
            .RuleFor(dr => dr.Description, f => f.Lorem.Sentence())
            .RuleFor(dr => dr.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(dr => dr.CreatedBy, f => f.Internet.UserName())
            .RuleFor(dr => dr.Name, (f, dr) =>
            {
                if (dr.IsFixedRole)
                {
                    return f.PickRandom(new[] { "db_owner", "db_accessadmin", "db_securityadmin", "db_ddladmin", "db_datawriter", "db_datareader", "db_denydatareader", "db_denydatawriter" });
                }
                return $"CustomDbRole_{f.Commerce.Department().Replace(" ", "")}";
            });

        var entities = faker.Generate(count);
        await this._context.DatabaseRoles.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Database Roles.");
    }

    private async Task SeedPermissions(int count, List<IAMBuddy.Tools.Data.Entities.Database> databases, List<DatabaseUser> databaseUsers, List<DatabaseRole> databaseRoles, List<ServerRole> serverRoles)
    {
        if (await this._context.Permissions.AnyAsync() || !databases.Any())
            return;

        var faker = new Faker<Permission>()
            .RuleFor(p => p.Id, f => 0)
            .RuleFor(p => p.PermissionName, f => f.PickRandom(new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "EXECUTE", "ALTER", "CONTROL" }))
            .RuleFor(p => p.Type, f => f.PickRandom<PermissionType>())
            .RuleFor(p => p.SecurableType, f => f.PickRandom<SecurableType>())
            .RuleFor(p => p.SecurableName, (f, p) => p.SecurableType switch
                {
                    SecurableType.Server => f.PickRandom(this._context.SqlServers.Select(s => s.Name).ToList()),
                    SecurableType.Database => f.PickRandom(this._context.Databases.Select(d => d.Name).ToList()),
                    SecurableType.Schema => f.Hacker.Noun(),
                    SecurableType.Table => f.Commerce.ProductName().Replace(" ", ""),
                    SecurableType.View => f.Hacker.Verb() + "View",
                    SecurableType.StoredProcedure => f.Hacker.Verb() + "Proc",
                    SecurableType.Function => f.Hacker.Adjective() + "Function",
                    SecurableType.Column => f.Commerce.ProductName().Replace(" ", "") + "Col",
                    _ => "UnknownSecurable"
                })
            .RuleFor(p => p.DatabaseId, (f, p) => f.PickRandom(databases).Id.OrNull(f, 0.2f))
            .RuleFor(p => p.DatabaseUserId, (f, p) => f.PickRandom(databaseUsers).Id.OrNull(f, 0.3f))
            .RuleFor(p => p.DatabaseRoleId, (f, p) => f.PickRandom(databaseRoles).Id.OrNull(f, 0.3f))
            .RuleFor(p => p.ServerRoleId, (f, p) => f.PickRandom(serverRoles).Id.OrNull(f, 0.1f))
            .RuleFor(p => p.GrantedBy, f => f.Internet.UserName())
            .RuleFor(p => p.GrantedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(p => p.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this._context.Permissions.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Permissions.");
    }

    private async Task SeedServerLoginRoles(int count, List<ServerLogin> serverLogins, List<ServerRole> serverRoles)
    {
        if (await this._context.ServerLoginRoles.AnyAsync() || !serverLogins.Any() || !serverRoles.Any())
            return;

        var faker = new Faker<ServerLoginRole>()
            .RuleFor(slr => slr.Id, f => 0)
            .RuleFor(slr => slr.ServerLoginId, f => f.PickRandom(serverLogins).Id)
            .RuleFor(slr => slr.ServerRoleId, f => f.PickRandom(serverRoles).Id)
            .RuleFor(slr => slr.AssignedDate, f => f.Date.Past(1))
            .RuleFor(slr => slr.AssignedBy, f => f.Internet.UserName())
            .RuleFor(slr => slr.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(slr => slr.CreatedBy, f => f.Internet.UserName());

        var entities = new List<ServerLoginRole>();
        var generatedCount = 0;
        while (generatedCount < count)
        {
            var slr = faker.Generate();
            // Ensure unique combination
            if (!entities.Any(e => e.ServerLoginId == slr.ServerLoginId && e.ServerRoleId == slr.ServerRoleId))
            {
                entities.Add(slr);
                generatedCount++;
            }
        }
        await this._context.ServerLoginRoles.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Server Login Roles.");
    }

    private async Task SeedDatabaseUserRoles(int count, List<DatabaseUser> databaseUsers, List<DatabaseRole> databaseRoles)
    {
        if (await this._context.DatabaseUserRoles.AnyAsync() || !databaseUsers.Any() || !databaseRoles.Any())
            return;

        var faker = new Faker<DatabaseUserRole>()
            .RuleFor(dur => dur.Id, f => 0)
            .RuleFor(dur => dur.DatabaseUserId, f => f.PickRandom(databaseUsers).Id)
            .RuleFor(dur => dur.DatabaseRoleId, f => f.PickRandom(databaseRoles).Id)
            .RuleFor(dur => dur.AssignedDate, f => f.Date.Past(1))
            .RuleFor(dur => dur.AssignedBy, f => f.Internet.UserName())
            .RuleFor(dur => dur.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(dur => dur.CreatedBy, f => f.Internet.UserName());

        var entities = new List<DatabaseUserRole>();
        var generatedCount = 0;
        while (generatedCount < count)
        {
            var dur = faker.Generate();
            // Ensure unique combination
            if (!entities.Any(e => e.DatabaseUserId == dur.DatabaseUserId && e.DatabaseRoleId == dur.DatabaseRoleId))
            {
                entities.Add(dur);
                generatedCount++;
            }
        }
        await this._context.DatabaseUserRoles.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Database User Roles.");
    }

    private async Task SeedAdminAuditLogs(int count)
    {
        if (await this._context.AdminAuditLogs.AnyAsync())
            return;

        var faker = new Faker<AdminAuditLog>()
            .RuleFor(aal => aal.Id, f => 0)
            .RuleFor(aal => aal.Action, f => f.PickRandom(new[] { "CREATE", "UPDATE", "DELETE", "GRANT", "REVOKE" }))
            .RuleFor(aal => aal.EntityType, f => f.PickRandom(new[] { "Server", "Database", "Login", "User", "Role", "Permission", "Application" }))
            .RuleFor(aal => aal.EntityId, f => f.Random.Int(1, 100)) // Assuming entity IDs range from 1 to 100 for dummy data
            .RuleFor(aal => aal.OldValues, f => f.Random.Bool(0.5f) ? System.Text.Json.JsonSerializer.Serialize(new { name = f.Name.FirstName() }) : null)
            .RuleFor(aal => aal.NewValues, f => f.Random.Bool(0.5f) ? System.Text.Json.JsonSerializer.Serialize(new { name = f.Name.FirstName() }) : null)
            .RuleFor(aal => aal.ActionDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(aal => aal.ActionBy, f => f.Internet.UserName())
            .RuleFor(aal => aal.Description, f => f.Lorem.Sentence());

        var entities = faker.Generate(count);
        await this._context.AdminAuditLogs.AddRangeAsync(entities);
        await this._context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Admin Audit Logs.");
    }

    // --- Seeding Default Values (for SQL Server defaults) ---

    public async Task SeedDefaultSqlServerValues()
    {
        Console.WriteLine("Seeding default SQL Server values...");

        // Ensure there's at least one SQL Server to attach defaults to
        if (!await this._context.SqlServers.AnyAsync())
        {
            await this.SeedSqlServers(1);
        }
        var defaultServer = await this._context.SqlServers.FirstAsync();

        // Default Logins (Example - typically 'sa' and built-in Windows groups)
        if (!await this._context.ServerLogins.AnyAsync(sl => sl.Name == "sa"))
        {
            this._context.ServerLogins.Add(new ServerLogin
            {
                ServerId = defaultServer.Id,
                Name = "sa",
                AuthenticationType = AuthenticationType.SqlServer,
                Status = LoginStatus.Active,
                DefaultDatabase = "master",
                DefaultLanguage = "us_english",
                PasswordExpirationDate = null, // Never expires for sa in real scenarios, or managed by policy
                IsPasswordExpired = false,
                IsLocked = false,
                LastLoginDate = DateTime.UtcNow,
                Description = "Default SQL Server System Administrator",
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            });
        }
        if (!await this._context.ServerLogins.AnyAsync(sl => sl.Name.Contains("BUILTIN\\Administrators")))
        {
            this._context.ServerLogins.Add(new ServerLogin
            {
                ServerId = defaultServer.Id,
                Name = "BUILTIN\\Administrators",
                AuthenticationType = AuthenticationType.Windows,
                Status = LoginStatus.Active,
                DefaultDatabase = "master",
                DefaultLanguage = "us_english",
                Description = "Windows Built-in Administrators Group",
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            });
        }
        // Add more default logins as needed (e.g., NT AUTHORITY\SYSTEM, NT SERVICE\MSSQLSERVER, etc.)

        // Default Server Roles
        var defaultServerRoles = new List<ServerRole>
        {
            new ServerRole { Name = "sysadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can perform any activity in the server.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "serveradmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can change server-wide configuration options and shut down the server.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "securityadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Manages logins and their properties. Grants, denies, or revokes server-level and database-level permissions.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "setupadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Adds and removes linked servers, and can start and stop extended stored procedures.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "processadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can terminate processes running in SQL Server.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "diskadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Manages disk files.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "dbcreator", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can create, alter, drop, and restore any database.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "bulkadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can run the BULK INSERT statement.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new ServerRole { Name = "public", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "All SQL Server logins are members of the public server role.", CreatedBy = "System", CreatedDate = DateTime.UtcNow }
        };

        foreach (var role in defaultServerRoles)
        {
            if (!await this._context.ServerRoles.AnyAsync(sr => sr.Name == role.Name && sr.ServerId == role.ServerId))
            {
                this._context.ServerRoles.Add(role);
            }
        }
        await this._context.SaveChangesAsync();

        // Default Databases and their Roles/Users/Permissions
        // This is a simplified example. In a real scenario, you'd enumerate actual default databases
        // like master, msdb, tempdb, model and seed their default users/roles/permissions.
        // For brevity, we'll just ensure a 'master' database is there and seed its defaults.

        var defaultInstance = await this._context.SqlServerInstances.FirstAsync(i => i.ServerId == defaultServer.Id);
        if (!await this._context.Databases.AnyAsync(d => d.Name == "master" && d.InstanceId == defaultInstance.Id))
        {
            var masterDb = new IAMBuddy.Tools.Data.Entities.Database
            {
                Name = "master",
                InstanceId = (await this._context.SqlServerInstances.FirstAsync(i => i.ServerId == defaultServer.Id)).Id,
                Collation = "SQL_Latin1_General_CP1_CI_AS",
                RecoveryModel = "SIMPLE",
                CompatibilityLevel = "160",
                IsActive = true,
                Description = "System database for managing SQL Server",
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };
            this._context.Databases.Add(masterDb);
            await this._context.SaveChangesAsync();

            // Default Database Roles for master
            var masterDbRoles = new List<DatabaseRole>
            {
                new DatabaseRole { Name = "public", DatabaseId = masterDb.Id, Type = RoleType.DatabaseRole, IsFixedRole = true, Description = "Default database role for all users.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                new DatabaseRole { Name = "db_owner", DatabaseId = masterDb.Id, Type = RoleType.DatabaseRole, IsFixedRole = true, Description = "All permissions in the database.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                // ... add other fixed database roles like db_datareader, db_datawriter etc.
            };
            foreach (var role in masterDbRoles)
            {
                if (!await this._context.DatabaseRoles.AnyAsync(dr => dr.Name == role.Name && dr.DatabaseId == role.DatabaseId))
                {
                    this._context.DatabaseRoles.Add(role);
                }
            }
            await this._context.SaveChangesAsync();

            // Default Database Users for master (e.g., dbo, guest)
            var masterDbUsers = new List<DatabaseUser>
            {
                new DatabaseUser { Name = "dbo", DatabaseId = masterDb.Id, ServerLoginId = (await this._context.ServerLogins.FirstAsync(sl => sl.Name == "sa")).Id, DefaultSchema = "dbo", UserType = "SQL_USER", IsActive = true, Description = "Database owner", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                new DatabaseUser { Name = "guest", DatabaseId = masterDb.Id, ServerLoginId = null, DefaultSchema = "guest", UserType = "SQL_USER", IsActive = true, Description = "Guest user", CreatedBy = "System", CreatedDate = DateTime.UtcNow }
            };
            foreach (var user in masterDbUsers)
            {
                if (!await this._context.DatabaseUsers.AnyAsync(du => du.Name == user.Name && du.DatabaseId == user.DatabaseId))
                {
                    this._context.DatabaseUsers.Add(user);
                }
            }
            await this._context.SaveChangesAsync();

            // Default Permissions for master (highly simplified, actual system permissions are complex)
            var dboUser = await this._context.DatabaseUsers.FirstAsync(du => du.Name == "dbo" && du.DatabaseId == masterDb.Id);
            var publicRole = await this._context.DatabaseRoles.FirstAsync(dr => dr.Name == "public" && dr.DatabaseId == masterDb.Id);

            var masterDbPermissions = new List<Permission>
            {
                new Permission { PermissionName = "CONTROL", Type = PermissionType.Grant, SecurableType = SecurableType.Database, SecurableName = masterDb.Name, DatabaseId = masterDb.Id, DatabaseUserId = dboUser.Id, GrantedBy = "System", GrantedDate = DateTime.UtcNow, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                new Permission { PermissionName = "CONNECT", Type = PermissionType.Grant, SecurableType = SecurableType.Database, SecurableName = masterDb.Name, DatabaseId = masterDb.Id, DatabaseRoleId = publicRole.Id, GrantedBy = "System", GrantedDate = DateTime.UtcNow, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                // Add more specific permissions as per SQL Server's default master DB permissions
            };
            foreach (var perm in masterDbPermissions)
            {
                // Simple check for uniqueness; real system permissions are complex to fully enumerate
                if (!await this._context.Permissions.AnyAsync(p => p.PermissionName == perm.PermissionName && p.SecurableName == perm.SecurableName && p.DatabaseId == perm.DatabaseId && p.DatabaseUserId == perm.DatabaseUserId && p.DatabaseRoleId == perm.DatabaseRoleId && p.ServerRoleId == perm.ServerRoleId))
                {
                    this._context.Permissions.Add(perm);
                }
            }
            await this._context.SaveChangesAsync();
        }

        Console.WriteLine("Seeding default SQL Server values completed.");
    }
}
