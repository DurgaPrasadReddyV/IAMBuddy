namespace IAMBuddy.Tools.Data.Seeding;
using Bogus;
using IAMBuddy.Tools.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class DataSeeder
{
    private readonly ToolsDbContext context;
    private static readonly string[] DataClassification = ["Public", "Internal", "Confidential", "Highly Confidential"];
    private static readonly string[] Versions = ["SQL Server 2019", "SQL Server 2022", "SQL Server 2017"];
    private static readonly string[] Editions = ["Enterprise", "Standard", "Developer"];
    private static readonly string[] ServicePacks = ["SP1", "SP2", "CU1", "RTM"];
    private static readonly string[] Ports = ["1433", "1434", "50000", "50001"];
    private static readonly string[] Collations = ["SQL_Latin1_General_CP1_CI_AS", "Latin1_General_CI_AS"];
    private static readonly string[] RecoveryModels = ["FULL", "SIMPLE", "BULK_LOGGED"];
    private static readonly string[] CompatibilityLevels = ["150", "160", "140"];
    private static readonly string[] Roles = ["Developer", "QA Engineer", "Product Manager", "Scrum Master", "Business Analyst"];
    private static readonly string[] AccessFrequencies = ["Daily", "Weekly", "Monthly", "Rarely"];
    private static readonly string[] Domains = ["corp.example.com", "dev.example.com", "contoso.local"];
    private static readonly string[] Resources = ["Server", "Database"];
    private static readonly string[] DefaultLanguages = ["us_english", "Japanese", "French"];
    private static readonly string[] DefaultSchemas = ["dbo", "app", "data"];
    private static readonly string[] RoleBasedSchemas = ["readonly", "app_user", "app_admin"];
    private static readonly string[] ServerRoles = ["sysadmin", "serveradmin", "securityadmin", "setupadmin", "processadmin", "diskadmin", "dbcreator", "bulkadmin", "public"];
    private static readonly string[] UserTypes = ["SQL_USER", "WINDOWS_USER", "WINDOWS_GROUP", "AZUREAD_USER", "AZUREAD_GROUP"];
    private static readonly string[] FixedRoles = ["db_owner", "db_accessadmin", "db_securityadmin", "db_ddladmin", "db_datawriter", "db_datareader", "db_denydatareader", "db_denydatawriter", "db_backupoperator"];
    private static readonly string[] Permissions = ["SELECT", "INSERT", "UPDATE", "DELETE", "EXECUTE", "ALTER", "CONTROL"];
    private static readonly string[] Actions = ["CREATE", "UPDATE", "DELETE", "GRANT", "REVOKE"];
    private static readonly string[] EntityTypes = ["Server", "Database", "Login", "User", "Role", "Permission", "Application"];

    public DataSeeder(ToolsDbContext context)
    {
        this.context = context;
        // Ensure that a consistent seed is used for repeatable results in development/testing
        Randomizer.Seed = new Random(8675309);
    }

    // Helper method to convert dates to UTC
    private static DateTime ToUtc(DateTime? dateTime) => dateTime?.ToUniversalTime() ?? DateTime.UtcNow;

    // Helper method to truncate strings to a specified max length
    private static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }

    public async Task SeedAllDummyData(SeedConfiguration config)
    {
        Console.WriteLine("Starting data seeding...");

        // Ensure database is created and migrations are applied
        await this.context.Database.MigrateAsync();

        // Seed core entities first as others depend on them
        var humanIdentities = await this.SeedHumanIdentities(config.HumanIdentityCount);
        var businessApplications = await this.SeedBusinessApplications(config.BusinessApplicationCount, humanIdentities);
        var sqlServers = await this.SeedSqlServers(config.SqlServerCount);
        var sqlServerInstances = await this.SeedSqlServerInstances(sqlServers);
        var databases = await this.SeedDatabases(sqlServerInstances);

        // Seed dependent entities
        await this.SeedBusinessApplicationEnvironments(config.BusinessApplicationEnvironmentCount, businessApplications);
        await this.SeedBusinessApplicationTeamMembers(config.BusinessApplicationTeamMemberCount, businessApplications, humanIdentities);
        await this.SeedNonHumanIdentities(config.NonHumanIdentityCount, businessApplications, humanIdentities, [.. this.context.BusinessApplicationEnvironments]); // Pass actual environments
        await this.SeedActiveDirectoryAccounts(config.ActiveDirectoryAccountCount, humanIdentities, [.. this.context.NonHumanIdentities]); // Pass actual non-human identities
        await this.SeedActiveDirectoryGroups(config.ActiveDirectoryGroupCount, [.. this.context.NonHumanIdentities]);
        await this.SeedActiveDirectoryGroupMemberships(config.ActiveDirectoryGroupMembershipCount, [.. this.context.ActiveDirectoryGroups], [.. this.context.ActiveDirectoryAccounts]);
        await this.SeedBusinessApplicationResources(config.BusinessApplicationResourceCount, businessApplications, sqlServers, databases);
        await this.SeedServerLogins(config.ServerLoginCount, sqlServers, [.. this.context.ActiveDirectoryAccounts], [.. this.context.ActiveDirectoryGroups], [.. this.context.NonHumanIdentities]);
        await this.SeedServerRoles(config.ServerRoleCount, sqlServers);
        await this.SeedDatabaseUsers(config.DatabaseUserCount, databases, [.. this.context.ServerLogins]);
        await this.SeedDatabaseRoles(config.DatabaseRoleCount, databases);
        await this.SeedPermissions(config.PermissionCount, databases, [.. this.context.DatabaseUsers], [.. this.context.DatabaseRoles], [.. this.context.ServerRoles]);
        await this.SeedServerLoginRoles(config.ServerLoginRoleCount, [.. this.context.ServerLogins], [.. this.context.ServerRoles]);
        await this.SeedDatabaseUserRoles(config.DatabaseUserRoleCount, [.. this.context.DatabaseUsers], [.. this.context.DatabaseRoles]);
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
        if (await this.context.HumanIdentities.AnyAsync())
        {
            return [];
        }

        var faker = new Faker<HumanIdentity>()
            .RuleFor(hi => hi.Id, f => 0) // Id will be set by EF Core
            .RuleFor(hi => hi.FirstName, f => Truncate(f.Name.FirstName(), 100))
            .RuleFor(hi => hi.LastName, f => Truncate(f.Name.LastName(), 100))
            .RuleFor(hi => hi.UserId, (f, hi) => Truncate($"{hi.FirstName.ToLowerInvariant()}.{hi.LastName.ToLowerInvariant()}{f.Random.Int(1, 100):00}", 100)) // meaningful userId
            .RuleFor(hi => hi.Email, (f, hi) => Truncate(f.Internet.Email(hi.FirstName, hi.LastName).ToLowerInvariant(), 255)) // meaningful email
            .RuleFor(hi => hi.Phone, f => Truncate(f.Phone.PhoneNumber(), 50))
            .RuleFor(hi => hi.JobTitle, f => Truncate(f.Name.JobTitle(), 100))
            .RuleFor(hi => hi.Department, f => Truncate(f.Commerce.Department(), 100))
            .RuleFor(hi => hi.Division, f => Truncate(f.Company.CompanyName(), 100)) // Changed from BsAdjective
            .RuleFor(hi => hi.CostCenter, f => Truncate(f.Finance.Account(6), 50))
            .RuleFor(hi => hi.Location, f => Truncate(f.Address.City(), 100))
            .RuleFor(hi => hi.Manager, f => Truncate(f.Name.FullName(), 100))
            .RuleFor(hi => hi.HireDate, f => ToUtc(f.Date.Past(10)))
            .RuleFor(hi => hi.TerminationDate, (f, hi) => ToUtc(f.Date.Between(hi.HireDate.GetValueOrDefault(), DateTime.UtcNow).OrNull(f, 0.2f)))
            .RuleFor(hi => hi.Status, f => f.PickRandom<HumanIdentityStatus>())
            .RuleFor(hi => hi.EmployeeId, f => Truncate(f.Finance.Account(8), 50))
            .RuleFor(hi => hi.Company, f => Truncate(f.Company.CompanyName(), 100))
            .RuleFor(hi => hi.IsContractor, f => f.Random.Bool(0.2f))
            .RuleFor(hi => hi.ContractEndDate, (f, hi) => hi.IsContractor ? ToUtc(f.Date.Future(2)) : null)
            .RuleFor(hi => hi.Description, f => Truncate(f.Lorem.Sentence(), 1000))
            .RuleFor(hi => hi.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(hi => hi.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this.context.HumanIdentities.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Human Identities.");
        return entities;
    }

    private async Task<List<BusinessApplication>> SeedBusinessApplications(int count, List<HumanIdentity> humanIdentities)
    {
        if (await this.context.BusinessApplications.AnyAsync() || humanIdentities.Count == 0)
        {
            return [];
        }

        var faker = new Faker<BusinessApplication>()
            .RuleFor(ba => ba.Id, f => 0)
            .RuleFor(ba => ba.Name, f => Truncate($"{f.Commerce.ProductName()} {f.Hacker.Noun()}", 255))
            .RuleFor(ba => ba.ShortName, (f, ba) => Truncate(string.Join("", ba.Name.Split(' ').Select(s => s[0]).Take(3)).ToUpperInvariant() + f.Random.Int(100, 999), 100))
            .RuleFor(ba => ba.Description, f => Truncate(f.Lorem.Sentence(5, 10), 1000))
            .RuleFor(ba => ba.BusinessPurpose, f => Truncate(f.Lorem.Paragraph(), 1000))
            .RuleFor(ba => ba.Status, f => f.PickRandom<BusinessApplicationStatus>())
            .RuleFor(ba => ba.Criticality, f => f.PickRandom<BusinessApplicationCriticality>())
            .RuleFor(ba => ba.ApplicationOwnerId, f => f.PickRandom(humanIdentities).Id)
            .RuleFor(ba => ba.AlternateApplicationOwnerId, (f, ba) => f.PickRandom(humanIdentities.Where(hi => hi.Id != ba.ApplicationOwnerId)).Id.OrNull(f, 0.3f))
            .RuleFor(ba => ba.ProductOwnerId, (f, ba) => f.PickRandom(humanIdentities).Id)
            .RuleFor(ba => ba.AlternateProductOwnerId, (f, ba) => f.PickRandom(humanIdentities.Where(hi => hi.Id != ba.ProductOwnerId)).Id.OrNull(f, 0.3f))
            .RuleFor(ba => ba.TechnicalContact, f => Truncate(f.Internet.Email(), 255))
            .RuleFor(ba => ba.BusinessContact, f => Truncate(f.Internet.Email(), 255))
            .RuleFor(ba => ba.VendorName, f => Truncate(f.Company.CompanyName(), 255))
            .RuleFor(ba => ba.Version, f => Truncate(f.System.Semver(), 50))
            .RuleFor(ba => ba.GoLiveDate, f => ToUtc(f.Date.Past(5)))
            .RuleFor(ba => ba.EndOfLifeDate, (f, ba) => ToUtc(f.Date.Future(5).OrNull(f, 0.1f)))
            .RuleFor(ba => ba.AnnualCost, f => f.Finance.Amount(1000, 100000))
            .RuleFor(ba => ba.ComplianceRequirements, f => Truncate(f.Lorem.Sentence(), 1000))
            .RuleFor(ba => ba.DataClassification, f => Truncate(f.PickRandom(DataClassification), 100))
            .RuleFor(ba => ba.IsCustomDeveloped, f => f.Random.Bool())
            .RuleFor(ba => ba.SourceCodeRepository, (f, ba) => ba.IsCustomDeveloped ? Truncate(f.Internet.UrlWithPath("github.com"), 500) : null)
            .RuleFor(ba => ba.DocumentationLink, f => Truncate(f.Internet.Url(), 500))
            .RuleFor(ba => ba.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ba => ba.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this.context.BusinessApplications.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Applications.");
        return entities;
    }

    private async Task<List<SqlServer>> SeedSqlServers(int count)
    {
        if (await this.context.SqlServers.AnyAsync())
        {
            return [];
        }

        var faker = new Faker<SqlServer>()
            .RuleFor(ss => ss.Id, f => 0)
            .RuleFor(ss => ss.Name, f => Truncate($"SQL-{f.Address.City().Replace(" ", "")}-{f.Random.Int(100, 999)}", 255))
            .RuleFor(ss => ss.HostName, (f, ss) => Truncate($"{ss.Name.ToLowerInvariant()}.example.com", 255))
            .RuleFor(ss => ss.IPAddress, f => Truncate(f.Internet.Ip(), 50))
            .RuleFor(ss => ss.Version, f => Truncate(f.PickRandom(Versions), 50))
            .RuleFor(ss => ss.Edition, f => Truncate(f.PickRandom(Editions), 100))
            .RuleFor(ss => ss.ServicePack, f => Truncate(f.PickRandom(ServicePacks), 50))
            .RuleFor(ss => ss.IsActive, f => f.Random.Bool())
            .RuleFor(ss => ss.Description, f => Truncate(f.Lorem.Sentence(), 500))
            .RuleFor(ss => ss.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ss => ss.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this.context.SqlServers.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} SQL Servers.");
        return entities;
    }

    private async Task<List<SqlServerInstance>> SeedSqlServerInstances(List<SqlServer> sqlServers)
    {
        if (await this.context.SqlServerInstances.AnyAsync() || sqlServers.Count == 0)
        {
            return [];
        }

        var possibleNames = new[] { "MSSQLSERVER", "PROD", "DEV", "UAT" };
        var entities = new List<SqlServerInstance>();
        var faker = new Faker<SqlServerInstance>()
            .RuleFor(ssi => ssi.Id, f => 0)
            .RuleFor(ssi => ssi.Port, f => Truncate(f.PickRandom(Ports), 10))
            .RuleFor(ssi => ssi.ServiceAccount, f => Truncate($"domain\\{f.Internet.UserName()}", 255))
            .RuleFor(ssi => ssi.Collation, f => Truncate(f.PickRandom(Collations), 100))
            .RuleFor(ssi => ssi.IsActive, f => f.Random.Bool())
            .RuleFor(ssi => ssi.Description, f => Truncate(f.Lorem.Sentence(), 500))
            .RuleFor(ssi => ssi.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(ssi => ssi.CreatedBy, f => f.Internet.UserName());

        var randomFaker = new Faker(); // For randomization

        foreach (var server in sqlServers)
        {
            // Generate 2 unique names per server
            var usedNames = new HashSet<string>();
            var instancesPerServer = 2;
            for (var i = 0; i < instancesPerServer; i++)
            {
                var baseName = randomFaker.PickRandom(possibleNames);
                var name = baseName;
                var suffix = 1;
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
        await this.context.SqlServerInstances.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} SQL Server Instances.");
        return entities;
    }

    private async Task<List<Entities.Database>> SeedDatabases(List<SqlServerInstance> sqlServerInstances)
    {
        if (await this.context.Databases.AnyAsync() || sqlServerInstances.Count == 0)
        {
            return [];
        }

        var faker = new Faker<Entities.Database>()
            .RuleFor(db => db.Id, f => 0)
            .RuleFor(db => db.InstanceId, f => f.PickRandom(sqlServerInstances).Id)
            .RuleFor(db => db.Name, f => Truncate(f.Database.Engine().Replace(" ", "") + f.Random.Int(100, 999), 255))
            .RuleFor(db => db.Collation, f => Truncate(f.PickRandom(Collations), 100))
            .RuleFor(db => db.RecoveryModel, f => Truncate(f.PickRandom(RecoveryModels), 50))
            .RuleFor(db => db.CompatibilityLevel, f => Truncate(f.PickRandom(CompatibilityLevels), 50))
            .RuleFor(db => db.IsActive, f => f.Random.Bool())
            .RuleFor(db => db.Description, f => Truncate(f.Lorem.Sentence(), 500))
            .RuleFor(db => db.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(db => db.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(sqlServerInstances.Count * 3); // 3 databases per instance on average
        await this.context.Databases.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Databases.");
        return entities;
    }

    private async Task SeedBusinessApplicationEnvironments(int count, List<BusinessApplication> businessApplications)
    {
        if (await this.context.BusinessApplicationEnvironments.AnyAsync() || businessApplications.Count == 0)
        {
            return;
        }

        var faker = new Faker<BusinessApplicationEnvironment>()
            .RuleFor(bae => bae.Id, f => 0)
            .RuleFor(bae => bae.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(bae => bae.Environment, f => f.PickRandom<ApplicationEnvironment>())
            .RuleFor(bae => bae.EnvironmentName, (f, bae) => Truncate(bae.Environment.ToString(), 100))
            .RuleFor(bae => bae.Description, f => Truncate(f.Lorem.Sentence(), 500))
            .RuleFor(bae => bae.IsActive, f => f.Random.Bool())
            .RuleFor(bae => bae.Url, (f, bae) =>
            {
                var businessApp = businessApplications.FirstOrDefault(ba => ba.Id == bae.BusinessApplicationId);
                return Truncate(f.Internet.UrlWithPath(businessApp?.ShortName?.ToLowerInvariant() ?? "app"), 500);
            })
            .RuleFor(bae => bae.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(bae => bae.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this.context.BusinessApplicationEnvironments.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Application Environments.");
    }

    private async Task SeedBusinessApplicationTeamMembers(int count, List<BusinessApplication> businessApplications, List<HumanIdentity> humanIdentities)
    {
        if (await this.context.BusinessApplicationTeamMembers.AnyAsync() || businessApplications.Count == 0 || humanIdentities.Count == 0)
        {
            return;
        }

        var faker = new Faker<BusinessApplicationTeamMember>()
            .RuleFor(batm => batm.Id, f => 0)
            .RuleFor(batm => batm.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(batm => batm.HumanIdentityId, f => f.PickRandom(humanIdentities).Id)
            .RuleFor(batm => batm.Role, f => Truncate(f.PickRandom(Roles), 100))
            .RuleFor(batm => batm.IsPrimary, f => f.Random.Bool())
            .RuleFor(batm => batm.StartDate, f => ToUtc(f.Date.Past(2)))
            .RuleFor(batm => batm.EndDate, (f, batm) => ToUtc(f.Date.Future(1).OrNull(f, 0.4f)))
            .RuleFor(batm => batm.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(batm => batm.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this.context.BusinessApplicationTeamMembers.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Application Team Members.");
    }

    private async Task SeedNonHumanIdentities(int count, List<BusinessApplication> businessApplications, List<HumanIdentity> humanIdentities, List<BusinessApplicationEnvironment> environments)
    {
        if (await this.context.NonHumanIdentities.AnyAsync() || businessApplications.Count == 0 || humanIdentities.Count == 0)
        {
            return;
        }

        var faker = new Faker<NonHumanIdentity>()
            .RuleFor(nhi => nhi.Id, f => 0)
            .RuleFor(nhi => nhi.Name, (f, nhi) => Truncate($"{f.System.CommonFileName(f.Random.Word())}-{f.Random.Int(100, 999)}", 255))
            .RuleFor(nhi => nhi.DisplayName, (f, nhi) => Truncate(nhi.Name.Replace("-", " "), 255))
            .RuleFor(nhi => nhi.Type, f => f.PickRandom<NonHumanIdentityType>())
            .RuleFor(nhi => nhi.Status, f => f.PickRandom<NonHumanIdentityStatus>())
            .RuleFor(nhi => nhi.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(nhi => nhi.BusinessApplicationEnvironmentId, (f, nhi) =>
            {
                var envs = environments.Where(e => e.BusinessApplicationId == nhi.BusinessApplicationId).ToList();
                if (envs.Count == 0)
                {
                    return null;
                }

                return f.PickRandom(envs).Id.OrNull(f, 0.3f);
            })
            .RuleFor(nhi => nhi.PrimaryOwnerId, f => f.PickRandom(humanIdentities).Id)
            .RuleFor(nhi => nhi.AlternateOwnerId, (f, nhi) => f.PickRandom(humanIdentities.Where(hi => hi.Id != nhi.PrimaryOwnerId)).Id.OrNull(f, 0.3f))
            .RuleFor(nhi => nhi.Purpose, f => Truncate(f.Lorem.Sentence(), 1000))
            .RuleFor(nhi => nhi.TechnicalContact, f => Truncate(f.Internet.Email(), 255))
            .RuleFor(nhi => nhi.ExpirationDate, f => ToUtc(f.Date.Future(3).OrNull(f, 0.2f)))
            .RuleFor(nhi => nhi.LastAccessDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(nhi => nhi.AccessFrequency, f => Truncate(f.PickRandom(AccessFrequencies), 50))
            .RuleFor(nhi => nhi.IsGeneric, f => f.Random.Bool(0.1f))
            .RuleFor(nhi => nhi.Description, f => Truncate(f.Lorem.Sentence(), 1000))
            .RuleFor(nhi => nhi.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(nhi => nhi.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this.context.NonHumanIdentities.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Non-Human Identities.");
    }

    private async Task SeedActiveDirectoryAccounts(int count, List<HumanIdentity> humanIdentities, List<NonHumanIdentity> nonHumanIdentities)
    {
        if (await this.context.ActiveDirectoryAccounts.AnyAsync() || (humanIdentities.Count == 0 && nonHumanIdentities.Count == 0))
        {
            return;
        }

        var faker = new Faker<ActiveDirectoryAccount>()
            .RuleFor(ada => ada.Id, f => 0)
            .RuleFor(ada => ada.AccountType, f => f.PickRandom<ActiveDirectoryAccountType>())
            .RuleFor(ada => ada.Domain, f => f.PickRandom(Domains))
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
                if (ada.AccountType == ActiveDirectoryAccountType.User && humanIdentities.Count != 0)
                {
                    return f.PickRandom(humanIdentities).Id;
                }
                return null;
            })
            .RuleFor(ada => ada.NonHumanIdentityId, (f, ada) =>
            {
                if (ada.AccountType == ActiveDirectoryAccountType.ServiceAccount && nonHumanIdentities.Count != 0)
                {
                    return f.PickRandom(nonHumanIdentities).Id;
                }
                return null;
            });

        var entities = new List<ActiveDirectoryAccount>();
        var samAccountNameCache = new HashSet<string>();
        var upnCache = new HashSet<string>();
        var distinguishedNameCache = new HashSet<string>();
        var randomizer = new Randomizer();

        var attempts = 0;
        var maxAttempts = count * 2; // Set a reasonable limit to prevent infinite loops

        while (entities.Count < count && attempts < maxAttempts)
        {
            var entity = faker.Generate();

            // Generate SamAccountName based on identity
            if (entity.HumanIdentityId.HasValue)
            {
                var human = humanIdentities.FirstOrDefault(hi => hi.Id == entity.HumanIdentityId.Value);
                var baseAccountName = $"{human?.FirstName.ToLowerInvariant() ?? randomizer.AlphaNumeric(5)}{human?.LastName.ToLowerInvariant()[..1] ?? randomizer.AlphaNumeric(1)}";
                var samAccountName = baseAccountName;
                var suffix = 1;
                while (samAccountNameCache.Contains(samAccountName))
                {
                    samAccountName = $"{baseAccountName}{suffix:000}";
                    suffix++;
                }
                entity.SamAccountName = Truncate(samAccountName, 255);
            }
            else if (entity.NonHumanIdentityId.HasValue)
            {
                var nonHuman = nonHumanIdentities.FirstOrDefault(nhi => nhi.Id == entity.NonHumanIdentityId.Value);
                var baseAccountName = $"svc_{nonHuman?.Name.ToLowerInvariant().Replace(" ", "") ?? randomizer.AlphaNumeric(8)}";
                var samAccountName = baseAccountName;
                var suffix = 1;
                while (samAccountNameCache.Contains(samAccountName))
                {
                    samAccountName = $"{baseAccountName}_{suffix:000}";
                    suffix++;
                }
                entity.SamAccountName = Truncate(samAccountName, 255);
            }
            else
            {
                var baseAccountName = $"generic_{randomizer.AlphaNumeric(8)}";
                var samAccountName = baseAccountName;
                var suffix = 1;
                while (samAccountNameCache.Contains(samAccountName))
                {
                    samAccountName = $"{baseAccountName}_{suffix:000}";
                    suffix++;
                }
                entity.SamAccountName = Truncate(samAccountName, 255);
            }

            // Generate UPN based on SamAccountName
            var upn = $"{entity.SamAccountName}@{entity.Domain}";
            // Set display name based on the account type
            if (entity.HumanIdentityId.HasValue)
            {
                var human = humanIdentities.FirstOrDefault(hi => hi.Id == entity.HumanIdentityId.Value);
                entity.DisplayName = Truncate($"{human?.FirstName} {human?.LastName}", 255);
            }
            else if (entity.NonHumanIdentityId.HasValue)
            {
                var nonHuman = nonHumanIdentities.FirstOrDefault(nhi => nhi.Id == entity.NonHumanIdentityId.Value);
                entity.DisplayName = Truncate(nonHuman?.DisplayName ?? entity.SamAccountName, 255);
            }
            else
            {
                entity.DisplayName = Truncate(entity.SamAccountName, 255);
            }

            // Generate unique distinguished name
            var baseDn = $"CN={entity.DisplayName},OU=Users,DC={entity.Domain.Replace(".", ",DC=")}";
            var dn = baseDn;
            var dnSuffix = 1;
            while (distinguishedNameCache.Contains(dn))
            {
                dn = $"CN={entity.DisplayName}_{dnSuffix},OU=Users,DC={entity.Domain.Replace(".", ",DC=")}";
                dnSuffix++;
            }
            entity.DistinguishedName = Truncate(dn, 1000);

            // Only add if all unique constraints are satisfied
            if (!samAccountNameCache.Contains(entity.SamAccountName) && !upnCache.Contains(upn) && !distinguishedNameCache.Contains(dn))
            {
                entity.UserPrincipalName = Truncate(upn, 255);
                samAccountNameCache.Add(entity.SamAccountName);
                upnCache.Add(upn);
                distinguishedNameCache.Add(dn);
                entities.Add(entity);
            }
            attempts++;
        }

        await this.context.ActiveDirectoryAccounts.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Active Directory Accounts.");
    }

    private async Task SeedActiveDirectoryGroups(int count, List<NonHumanIdentity> nonHumanIdentities)
    {
        if (await this.context.ActiveDirectoryGroups.AnyAsync() || nonHumanIdentities.Count == 0)
        {
            return;
        }

        var faker = new Faker<ActiveDirectoryGroup>()
            .RuleFor(dr => dr.Id, f => 0)
            .RuleFor(dr => dr.GroupType, f => f.PickRandom<ActiveDirectoryGroupType>())
            .RuleFor(dr => dr.GroupScope, f => f.PickRandom<ActiveDirectoryGroupScope>())
            .RuleFor(dr => dr.Domain, f => f.PickRandom(Domains))
            .RuleFor(dr => dr.IsActive, f => f.Random.Bool())
            .RuleFor(dr => dr.ManagedBy, (f, dr) => f.Random.Bool(0.7f) && nonHumanIdentities.Count != 0 ? f.PickRandom(nonHumanIdentities).Name : f.Internet.UserName())
            .RuleFor(dr => dr.NonHumanIdentityId, (f, dr) => f.Random.Bool(0.7f) && nonHumanIdentities.Count != 0 ? f.PickRandom(nonHumanIdentities).Id : null)
            .RuleFor(dr => dr.Description, f => f.Lorem.Sentence())
            .RuleFor(dr => dr.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(dr => dr.CreatedBy, f => f.Internet.UserName())
            .RuleFor(dr => dr.Name, (f, dr) =>
            {
                var typePrefix = dr.GroupType == ActiveDirectoryGroupType.Security ? "SG" : "DL";
                var scopeSuffix = dr.GroupScope switch
                {
                    ActiveDirectoryGroupScope.DomainLocal => "DL",
                    ActiveDirectoryGroupScope.Global => "G",
                    ActiveDirectoryGroupScope.Universal => "U",
                    _ => ""
                };
                return Truncate($"{typePrefix}_{f.Commerce.Department()}_{scopeSuffix}_{f.Random.Int(100, 999)}".Replace(" ", "_"), 255);
            });

        var entities = new List<ActiveDirectoryGroup>();
        var usedNames = new HashSet<string>();
        var usedSamAccountNames = new HashSet<string>();
        var usedDistinguishedNames = new HashSet<string>();
        var generatedCount = 0;
        var maxAttempts = count * 3; // Set a reasonable limit to prevent infinite loops
        var attempts = 0;

        while (generatedCount < count && attempts < maxAttempts)
        {
            var group = faker.Generate();
            attempts++;

            // Generate SamAccountName based on the Name
            var samAccountName = group.Name;
            var samSuffix = 1;
            while (usedSamAccountNames.Contains(samAccountName))
            {
                samAccountName = $"{group.Name}_{samSuffix:000}";
                samSuffix++;
            }

            // Ensure unique name
            if (usedNames.Contains(group.Name))
            {
                continue;
            }

            // Create distinguished name with unique components
            var domainComponents = group.Domain.Split('.').Select(dc => $"DC={dc}");
            var ouPath = $"OU=Groups,OU={group.GroupType},OU={group.GroupScope}";
            var baseDn = $"CN={group.Name},{ouPath},{string.Join(",", domainComponents)}";
            var dn = baseDn;
            var dnSuffix = 1;

            while (usedDistinguishedNames.Contains(dn))
            {
                dn = $"CN={group.Name}_{dnSuffix:000},{ouPath},{string.Join(",", domainComponents)}";
                dnSuffix++;
            }

            // Set the final values
            group.SamAccountName = Truncate(samAccountName, 255);
            group.DisplayName = Truncate(group.Name.Replace("_", " "), 255);
            group.DistinguishedName = Truncate(dn, 1000);
            group.Email = Truncate($"{group.SamAccountName}@{group.Domain}".ToLowerInvariant(), 255);

            // Add to tracking sets
            usedNames.Add(group.Name);
            usedSamAccountNames.Add(group.SamAccountName);
            usedDistinguishedNames.Add(group.DistinguishedName);
            entities.Add(group);
            generatedCount++;
        }

        if (entities.Count != 0)
        {
            await this.context.ActiveDirectoryGroups.AddRangeAsync(entities);
            await this.context.SaveChangesAsync();
            Console.WriteLine($"Seeded {entities.Count} Active Directory Groups.");
        }
        else
        {
            Console.WriteLine("Warning: No Active Directory Groups were seeded due to uniqueness constraints.");
        }
    }

    private async Task SeedActiveDirectoryGroupMemberships(int count, List<ActiveDirectoryGroup> groups, List<ActiveDirectoryAccount> accounts)
    {
        if (await this.context.ActiveDirectoryGroupMemberships.AnyAsync() || groups.Count == 0 || accounts.Count == 0)
        {
            return;
        }

        var faker = new Faker<ActiveDirectoryGroupMembership>()
            .RuleFor(adgm => adgm.Id, f => 0)
            .RuleFor(adgm => adgm.GroupId, f => f.PickRandom(groups).Id)
            .RuleFor(adgm => adgm.AccountId, (f, adgm) => f.Random.Bool(0.7f) && accounts.Count != 0 ? f.PickRandom(accounts).Id : null)
            .RuleFor(adgm => adgm.ChildGroupId, (f, adgm) => f.Random.Bool(0.3f) && groups.Count != 0 ? f.PickRandom(groups.Where(g => g.Id != adgm.GroupId)).Id : null) // Ensure no self-referencing
            .RuleFor(adgm => adgm.MemberSince, f => ToUtc(f.Date.Past(1)))
            .RuleFor(adgm => adgm.AddedBy, f => f.Internet.UserName())
            .RuleFor(adgm => adgm.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(adgm => adgm.CreatedBy, f => f.Internet.UserName());

        // Ensure at least one of AccountId or ChildGroupId is set
        var entities = new List<ActiveDirectoryGroupMembership>();
        var uniqueMemberships = new HashSet<(int, int?, int?)>();
        var generatedCount = 0;
        while (generatedCount < count)
        {
            var membership = faker.Generate();
            if (membership.AccountId.HasValue || membership.ChildGroupId.HasValue)
            {
                var uniqueTuple = (membership.GroupId, membership.AccountId, membership.ChildGroupId);
                // Prevent duplicate memberships based on GroupId, AccountId, ChildGroupId combination
                if (!uniqueMemberships.Contains(uniqueTuple))
                {
                    entities.Add(membership);
                    uniqueMemberships.Add(uniqueTuple);
                    generatedCount++;
                }
            }
        }

        await this.context.ActiveDirectoryGroupMemberships.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Active Directory Group Memberships.");
    }

    private async Task SeedBusinessApplicationResources(int count, List<BusinessApplication> businessApplications, List<SqlServer> sqlServers, List<Entities.Database> databases)
    {
        if (await this.context.BusinessApplicationResources.AnyAsync() || businessApplications.Count == 0 || sqlServers.Count == 0 || databases.Count == 0)
        {
            return;
        }

        var faker = new Faker<BusinessApplicationResource>()
            .RuleFor(bar => bar.Id, f => 0)
            .RuleFor(bar => bar.BusinessApplicationId, f => f.PickRandom(businessApplications).Id)
            .RuleFor(bar => bar.ResourceType, f => Truncate(f.PickRandom(Resources), 100)) // Extend as needed
            .RuleFor(bar => bar.Environment, f => f.PickRandom<ApplicationEnvironment>())
            .RuleFor(bar => bar.Purpose, f => Truncate(f.Lorem.Sentence(), 500))
            .RuleFor(bar => bar.IsCritical, f => f.Random.Bool(0.3f))
            .RuleFor(bar => bar.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(bar => bar.CreatedBy, f => f.Internet.UserName())
            .RuleFor(bar => bar.ResourceId, (f, bar) =>
            {
                if (bar.ResourceType == "Server" && sqlServers.Count != 0)
                {
                    return f.PickRandom(sqlServers).Id;
                }
                else if (bar.ResourceType == "Database" && databases.Count != 0)
                {
                    return f.PickRandom(databases).Id;
                }
                return 0; // Will be filtered out or throw if 0 is not valid
            })
            .RuleFor(bar => bar.ResourceName, (f, bar) =>
            {
                if (bar.ResourceType == "Server" && sqlServers.Count != 0)
                {
                    return Truncate(sqlServers.FirstOrDefault(s => s.Id == bar.ResourceId)?.Name, 255);
                }
                else if (bar.ResourceType == "Database" && databases.Count != 0)
                {
                    return Truncate(databases.FirstOrDefault(d => d.Id == bar.ResourceId)?.Name, 255);
                }
                return null;
            });

        var entities = faker.Generate(count).Where(e => e.ResourceId != 0).ToList(); // Filter out entities where ResourceId wasn't set correctly
        await this.context.BusinessApplicationResources.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Business Application Resources.");
    }

    private async Task SeedServerLogins(int count, List<SqlServer> sqlServers, List<ActiveDirectoryAccount> adAccounts, List<ActiveDirectoryGroup> adGroups, List<NonHumanIdentity> nonHumanIdentities)
    {
        if (await this.context.ServerLogins.AnyAsync() || sqlServers.Count == 0 || adAccounts.Count == 0 || adGroups.Count == 0 || nonHumanIdentities.Count == 0)
        {
            return;
        }

        var loginNamesPerServer = new Dictionary<int, HashSet<string>>(); // Track used names per server
        foreach (var server in sqlServers)
        {
            loginNamesPerServer[server.Id] = [];
        }

        var f = new Faker(); // Create a Faker instance for random operations
        var entities = new List<ServerLogin>();
        var attempts = 0;
        var maxAttempts = count * 3; // Set a reasonable limit to prevent infinite loops

        while (entities.Count < count && attempts < maxAttempts)
        {
            attempts++;
            var serverId = f.PickRandom(sqlServers).Id;
            var authenticationType = f.PickRandom<AuthenticationType>();

            var entity = new ServerLogin
            {
                ServerId = serverId,
                AuthenticationType = authenticationType,
                Status = f.PickRandom<LoginStatus>(),
                DefaultDatabase = f.Random.Bool(0.8f) ? f.Database.Engine() : null,
                DefaultLanguage = f.PickRandom(DefaultLanguages),
                PasswordExpirationDate = ToUtc(f.Date.Future(1).OrNull(f, 0.3f)),
                LastLoginDate = ToUtc(f.Date.Recent()),
                CreatedDate = ToUtc(f.Date.Past(1)),
                CreatedBy = f.Internet.UserName()
            };

            // Determine identity type and set properties accordingly
            if (authenticationType == AuthenticationType.SqlServer)
            {
                if (f.Random.Bool(0.2f) && nonHumanIdentities.Count != 0)
                {
                    var nhi = f.PickRandom(nonHumanIdentities);
                    entity.NonHumanIdentityId = nhi.Id;
                    entity.Name = $"SQL_{nhi.Name}_{f.Random.AlphaNumeric(4)}".Replace(" ", "_");
                }
                else
                {
                    entity.Name = $"SQL_{f.Internet.UserName()}_{f.Random.AlphaNumeric(4)}";
                }
            }
            else if (authenticationType is AuthenticationType.Windows or AuthenticationType.AzureAD)
            {
                if (f.Random.Bool(0.7f)) // 70% chance of being a user account
                {
                    var adAccount = f.PickRandom(adAccounts);
                    entity.ActiveDirectoryAccountId = adAccount.Id;
                    entity.Name = adAccount.SamAccountName;
                }
                else // 30% chance of being a group
                {
                    var adGroup = f.PickRandom(adGroups);
                    entity.ActiveDirectoryGroupId = adGroup.Id;
                    entity.Name = adGroup.SamAccountName;
                }
            }

            // Ensure the name is unique for this server
            if (!string.IsNullOrEmpty(entity.Name) && !loginNamesPerServer[serverId].Contains(entity.Name))
            {
                entity.Description = $"{entity.AuthenticationType} login for {entity.Name}";
                entity.IsPasswordExpired = entity.PasswordExpirationDate.HasValue && entity.PasswordExpirationDate < DateTime.Now;
                entity.IsLocked = entity.Status == LoginStatus.Locked;
                loginNamesPerServer[serverId].Add(entity.Name);
                entities.Add(entity);
            }
        }

        if (entities.Count < count)
        {
            Console.WriteLine($"Warning: Could only generate {entities.Count} unique server logins out of {count} requested.");
        }

        await this.context.ServerLogins.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Server Logins.");
    }

    private async Task SeedServerRoles(int count, List<SqlServer> sqlServers)
    {
        if (await this.context.ServerRoles.AnyAsync() || sqlServers.Count == 0)
        {
            return;
        }

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
                    return Truncate(f.PickRandom(ServerRoles), 255);
                }
                return Truncate($"CustomServerRole_{f.Commerce.Department().Replace(" ", "")}", 255);
            });

        var entities = faker.Generate(count);
        await this.context.ServerRoles.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Server Roles.");
    }

    private async Task SeedDatabaseUsers(int count, List<Entities.Database> databases, List<ServerLogin> serverLogins)
    {
        if (await this.context.DatabaseUsers.AnyAsync() || databases.Count == 0 || serverLogins.Count == 0)
        {
            return;
        }

        var faker = new Faker<DatabaseUser>()
            .RuleFor(du => du.Id, f => 0)
            .RuleFor(du => du.DatabaseId, f => f.PickRandom(databases).Id)
            .RuleFor(du => du.UserType, f => f.PickRandom(UserTypes))
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
                    _ => []
                };

                return validLogins.Any() ? f.PickRandom(validLogins).Id : null;
            })
            .RuleFor(du => du.DefaultSchema, (f, du) => du.UserType switch
            {
                "SQL_USER" => "dbo", // SQL users often use dbo schema
                "WINDOWS_USER" or "AZUREAD_USER" => f.PickRandom(DefaultSchemas), // Individual users might have specific schemas
                "WINDOWS_GROUP" or "AZUREAD_GROUP" => f.PickRandom(RoleBasedSchemas), // Groups often have role-based schemas
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
                        return Truncate(name, 255);
                    }
                }
                // Generate a meaningful name for users without logins
                return Truncate($"{du.UserType.ToLowerInvariant()}_{f.Internet.UserName()}", 255);
            });

        var entities = faker.Generate(count)
            // Ensure we have unique names per database
            .GroupBy(du => new { du.DatabaseId, du.Name })
            .Select(g => g.First())
            .ToList();

        await this.context.DatabaseUsers.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Database Users.");
    }

    private async Task SeedDatabaseRoles(int count, List<Entities.Database> databases)
    {
        if (await this.context.DatabaseRoles.AnyAsync() || databases.Count == 0)
        {
            return;
        }

        var roleNamesPerDb = new Dictionary<int, HashSet<string>>();
        foreach (var db in databases)
        {
            roleNamesPerDb[db.Id] = [];
        }

        var faker = new Faker<DatabaseRole>()
            .RuleFor(dr => dr.Id, f => 0)
            .RuleFor(dr => dr.DatabaseId, f => f.PickRandom(databases).Id)
            .RuleFor(dr => dr.Type, f => RoleType.DatabaseRole)
            .RuleFor(dr => dr.IsFixedRole, f => f.Random.Bool(0.5f))
            .RuleFor(dr => dr.Description, f => f.Lorem.Sentence())
            .RuleFor(dr => dr.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(dr => dr.CreatedBy, f => f.Internet.UserName())
            .RuleFor(dr => dr.Name, (f, dr) =>
            {
                string roleName;
                if (dr.IsFixedRole)
                {
                    // Try to find an unused fixed role name
                    roleName = f.PickRandom(FixedRoles.Where(r => !roleNamesPerDb[dr.DatabaseId].Contains(r)).ToList());

                    // If all fixed roles are used, create a unique custom role
                    if (string.IsNullOrEmpty(roleName))
                    {
                        dr.IsFixedRole = false; // Convert to custom role
                        roleName = GenerateUniqueRoleName(f, dr.DatabaseId, roleNamesPerDb);
                    }
                }
                else
                {
                    roleName = GenerateUniqueRoleName(f, dr.DatabaseId, roleNamesPerDb);
                }

                roleNamesPerDb[dr.DatabaseId].Add(roleName);
                return Truncate(roleName, 255);
            });

        var entities = new List<DatabaseRole>();
        var attempts = 0;
        var maxAttempts = count * 3;

        while (entities.Count < count && attempts < maxAttempts)
        {
            attempts++;
            var role = faker.Generate();

            // Skip if we couldn't generate a unique name
            if (string.IsNullOrEmpty(role.Name))
            {
                continue;
            }

            entities.Add(role);
        }

        if (entities.Count != 0)
        {
            await this.context.DatabaseRoles.AddRangeAsync(entities);
            await this.context.SaveChangesAsync();
            Console.WriteLine($"Seeded {entities.Count} Database Roles.");
        }
        else
        {
            Console.WriteLine("Warning: No Database Roles were seeded due to uniqueness constraints.");
        }
    }

    // Helper method to generate unique role names
    private static string GenerateUniqueRoleName(Faker f, int databaseId, Dictionary<int, HashSet<string>> roleNamesPerDb)
    {
        var maxAttempts = 100;
        var attempts = 0;
        string roleName;

        do
        {
            attempts++;
            var department = f.Commerce.Department().Replace(" ", "");
            var suffix = attempts > 1 ? $"_{attempts:000}" : "";
            roleName = $"CustomDbRole_{department}{suffix}";
        }
        while (roleNamesPerDb[databaseId].Contains(roleName) && attempts < maxAttempts);

        return attempts < maxAttempts ? roleName : string.Empty;
    }

    private async Task SeedPermissions(int count, List<Entities.Database> databases, List<DatabaseUser> databaseUsers, List<DatabaseRole> databaseRoles, List<ServerRole> serverRoles)
    {
        if (await this.context.Permissions.AnyAsync() || databases.Count == 0 || databaseUsers.Count == 0 || databaseRoles.Count == 0 || serverRoles.Count == 0)
        {
            return;
        }

        var faker = new Faker<DatabasePermission>()
            .RuleFor(p => p.Id, f => 0)
            .RuleFor(p => p.PermissionName, f => Truncate(f.PickRandom(Permissions), 255))
            .RuleFor(p => p.Type, f => f.PickRandom<PermissionType>())
            .RuleFor(p => p.SecurableType, f => f.PickRandom<SecurableType>())
            .RuleFor(p => p.SecurableName, (f, p) => Truncate(p.SecurableType switch
            {
                SecurableType.Server => f.PickRandom(this.context.SqlServers.Select(s => s.Name).ToList()),
                SecurableType.Database => f.PickRandom(this.context.Databases.Select(d => d.Name).ToList()),
                SecurableType.Schema => f.Hacker.Noun(),
                SecurableType.Table => f.Commerce.ProductName().Replace(" ", ""),
                SecurableType.View => f.Hacker.Verb() + "View",
                SecurableType.StoredProcedure => f.Hacker.Verb() + "Proc",
                SecurableType.Function => f.Hacker.Adjective() + "Function",
                SecurableType.Column => f.Commerce.ProductName().Replace(" ", "") + "Col",
                _ => "UnknownSecurable"
            }, 255))
            .RuleFor(p => p.DatabaseId, (f, p) => f.PickRandom(databases).Id.OrNull(f, 0.2f))
            .RuleFor(p => p.DatabaseUserId, (f, p) => f.PickRandom(databaseUsers).Id.OrNull(f, 0.3f))
            .RuleFor(p => p.DatabaseRoleId, (f, p) => f.PickRandom(databaseRoles).Id.OrNull(f, 0.3f))
            .RuleFor(p => p.ServerRoleId, (f, p) => f.PickRandom(serverRoles).Id.OrNull(f, 0.1f))
            .RuleFor(p => p.GrantedBy, f => Truncate(f.Internet.UserName(), 255))
            .RuleFor(p => p.GrantedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(p => p.Description, f => Truncate(f.Lorem.Sentence(), 1000))
            .RuleFor(p => p.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(p => p.CreatedBy, f => f.Internet.UserName());

        var entities = faker.Generate(count);
        await this.context.Permissions.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Permissions.");
    }

    private async Task SeedServerLoginRoles(int count, List<ServerLogin> serverLogins, List<ServerRole> serverRoles)
    {
        if (await this.context.ServerLoginRoles.AnyAsync() || serverLogins.Count == 0 || serverRoles.Count == 0)
        {
            return;
        }

        var faker = new Faker<ServerLoginRole>()
            .RuleFor(slr => slr.Id, f => 0)
            .RuleFor(slr => slr.ServerLoginId, f => f.PickRandom(serverLogins).Id)
            .RuleFor(slr => slr.ServerRoleId, f => f.PickRandom(serverRoles).Id)
            .RuleFor(slr => slr.AssignedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(slr => slr.AssignedBy, f => f.Internet.UserName())
            .RuleFor(slr => slr.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(slr => slr.CreatedBy, f => f.Internet.UserName());

        var entities = new List<ServerLoginRole>();
        var uniqueRoles = new HashSet<(int, int)>();
        var generatedCount = 0;
        while (generatedCount < count)
        {
            var slr = faker.Generate();
            var uniqueTuple = (slr.ServerLoginId, slr.ServerRoleId);
            // Ensure unique combination
            if (!uniqueRoles.Contains(uniqueTuple))
            {
                entities.Add(slr);
                uniqueRoles.Add(uniqueTuple);
                generatedCount++;
            }
        }
        await this.context.ServerLoginRoles.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Server Login Roles.");
    }

    private async Task SeedDatabaseUserRoles(int count, List<DatabaseUser> databaseUsers, List<DatabaseRole> databaseRoles)
    {
        if (await this.context.DatabaseUserRoles.AnyAsync() || databaseUsers.Count == 0 || databaseRoles.Count == 0)
        {
            return;
        }

        var faker = new Faker<DatabaseUserRole>()
            .RuleFor(dur => dur.Id, f => 0)
            .RuleFor(dur => dur.DatabaseUserId, f => f.PickRandom(databaseUsers).Id)
            .RuleFor(dur => dur.DatabaseRoleId, f => f.PickRandom(databaseRoles).Id)
            .RuleFor(dur => dur.AssignedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(dur => dur.AssignedBy, f => f.Internet.UserName())
            .RuleFor(dur => dur.CreatedDate, f => ToUtc(f.Date.Past(1)))
            .RuleFor(dur => dur.CreatedBy, f => f.Internet.UserName());

        var entities = new List<DatabaseUserRole>();
        var uniqueRoles = new HashSet<(int, int)>();
        var generatedCount = 0;
        while (generatedCount < count)
        {
            var dur = faker.Generate();
            var uniqueTuple = (dur.DatabaseUserId, dur.DatabaseRoleId);
            // Ensure unique combination
            if (!uniqueRoles.Contains(uniqueTuple))
            {
                entities.Add(dur);
                uniqueRoles.Add(uniqueTuple);
                generatedCount++;
            }
        }
        await this.context.DatabaseUserRoles.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Database User Roles.");
    }

    private async Task SeedAdminAuditLogs(int count)
    {
        if (await this.context.AdminAuditLogs.AnyAsync())
        {
            return;
        }

        var faker = new Faker<AdminAuditLog>()
            .RuleFor(aal => aal.Id, f => 0)
            .RuleFor(aal => aal.Action, f => Truncate(f.PickRandom(Actions), 255))
            .RuleFor(aal => aal.EntityType, f => Truncate(f.PickRandom(EntityTypes), 255))
            .RuleFor(aal => aal.EntityId, f => f.Random.Int(1, 100)) // Assuming entity IDs range from 1 to 100 for dummy data
            .RuleFor(aal => aal.OldValues, f => f.Random.Bool(0.5f) ? System.Text.Json.JsonSerializer.Serialize(new { name = f.Name.FirstName() }) : null)
            .RuleFor(aal => aal.NewValues, f => f.Random.Bool(0.5f) ? System.Text.Json.JsonSerializer.Serialize(new { name = f.Name.FirstName() }) : null)
            .RuleFor(aal => aal.ActionDate, f => ToUtc(f.Date.Past(1))) // Convert to UTC
            .RuleFor(aal => aal.ActionBy, f => Truncate(f.Internet.UserName(), 255))
            .RuleFor(aal => aal.Description, f => Truncate(f.Lorem.Sentence(), 1000));

        var entities = faker.Generate(count);
        await this.context.AdminAuditLogs.AddRangeAsync(entities);
        await this.context.SaveChangesAsync();
        Console.WriteLine($"Seeded {entities.Count} Admin Audit Logs.");
    }

    // --- Seeding Default Values (for SQL Server defaults) ---

    public async Task SeedDefaultSqlServerValues()
    {
        Console.WriteLine("Seeding default SQL Server values...");

        // Ensure there's at least one SQL Server to attach defaults to
        if (!await this.context.SqlServers.AnyAsync())
        {
            await this.SeedSqlServers(1);
        }
        var defaultServer = await this.context.SqlServers.FirstAsync();

        // Default Logins (Example - typically 'sa' and built-in Windows groups)
        if (!await this.context.ServerLogins.AnyAsync(sl => sl.Name == "sa"))
        {
            this.context.ServerLogins.Add(new ServerLogin
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
        if (!await this.context.ServerLogins.AnyAsync(sl => sl.Name.Contains("BUILTIN\\Administrators")))
        {
            this.context.ServerLogins.Add(new ServerLogin
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
            new() { Name = "sysadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can perform any activity in the server.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "serveradmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can change server-wide configuration options and shut down the server.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "securityadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Manages logins and their properties. Grants, denies, or revokes server-level and database-level permissions.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "setupadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Adds and removes linked servers, and can start and stop extended stored procedures.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "processadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can terminate processes running in SQL Server.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "diskadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Manages disk files.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "dbcreator", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can create, alter, drop, and restore any database.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "bulkadmin", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "Can run the BULK INSERT statement.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
            new() { Name = "public", ServerId = defaultServer.Id, Type = RoleType.ServerRole, IsFixedRole = true, Description = "All SQL Server logins are members of the public server role.", CreatedBy = "System", CreatedDate = DateTime.UtcNow }
        };

        foreach (var role in defaultServerRoles)
        {
            if (!await this.context.ServerRoles.AnyAsync(sr => sr.Name == role.Name && sr.ServerId == role.ServerId))
            {
                this.context.ServerRoles.Add(role);
            }
        }
        await this.context.SaveChangesAsync();

        // Default Databases and their Roles/Users/Permissions
        // This is a simplified example. In a real scenario, you'd enumerate actual default databases
        // like master, msdb, tempdb, model and seed their default users/roles/permissions.
        // For brevity, we'll just ensure a 'master' database is there and seed its defaults.

        var defaultInstance = await this.context.SqlServerInstances.FirstAsync(i => i.ServerId == defaultServer.Id);
        if (!await this.context.Databases.AnyAsync(d => d.Name == "master" && d.InstanceId == defaultInstance.Id))
        {
            var masterDb = new Entities.Database
            {
                Name = "master",
                InstanceId = (await this.context.SqlServerInstances.FirstAsync(i => i.ServerId == defaultServer.Id)).Id,
                Collation = "SQL_Latin1_General_CP1_CI_AS",
                RecoveryModel = "SIMPLE",
                CompatibilityLevel = "160",
                IsActive = true,
                Description = "System database for managing SQL Server",
                CreatedBy = "System",
                CreatedDate = DateTime.UtcNow
            };
            this.context.Databases.Add(masterDb);
            await this.context.SaveChangesAsync();

            // Default Database Roles for master
            var masterDbRoles = new List<DatabaseRole>
            {
                new() { Name = "public", DatabaseId = masterDb.Id, Type = RoleType.DatabaseRole, IsFixedRole = true, Description = "Default database role for all users.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                new() { Name = "db_owner", DatabaseId = masterDb.Id, Type = RoleType.DatabaseRole, IsFixedRole = true, Description = "All permissions in the database.", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                // ... add other fixed database roles like db_datareader, db_datawriter etc.
            };
            foreach (var role in masterDbRoles)
            {
                if (!await this.context.DatabaseRoles.AnyAsync(dr => dr.Name == role.Name && dr.DatabaseId == role.DatabaseId))
                {
                    this.context.DatabaseRoles.Add(role);
                }
            }
            await this.context.SaveChangesAsync();

            // Default Database Users for master (e.g., dbo, guest)
            var masterDbUsers = new List<DatabaseUser>
            {
                new() { Name = "dbo", DatabaseId = masterDb.Id, ServerLoginId = (await this.context.ServerLogins.FirstAsync(sl => sl.Name == "sa")).Id, DefaultSchema = "dbo", UserType = "SQL_USER", IsActive = true, Description = "Database owner", CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                new() { Name = "guest", DatabaseId = masterDb.Id, ServerLoginId = null, DefaultSchema = "guest", UserType = "SQL_USER", IsActive = true, Description = "Guest user", CreatedBy = "System", CreatedDate = DateTime.UtcNow }
            };
            foreach (var user in masterDbUsers)
            {
                if (!await this.context.DatabaseUsers.AnyAsync(du => du.Name == user.Name && du.DatabaseId == user.DatabaseId))
                {
                    this.context.DatabaseUsers.Add(user);
                }
            }
            await this.context.SaveChangesAsync();

            // Default Permissions for master (highly simplified, actual system permissions are complex)
            var dboUser = await this.context.DatabaseUsers.FirstAsync(du => du.Name == "dbo" && du.DatabaseId == masterDb.Id);
            var publicRole = await this.context.DatabaseRoles.FirstAsync(dr => dr.Name == "public" && dr.DatabaseId == masterDb.Id);

            var masterDbPermissions = new List<DatabasePermission>
            {
                new() { PermissionName = "CONTROL", Type = PermissionType.Grant, SecurableType = SecurableType.Database, SecurableName = masterDb.Name, DatabaseId = masterDb.Id, DatabaseUserId = dboUser.Id, GrantedBy = "System", GrantedDate = DateTime.UtcNow, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                new() { PermissionName = "CONNECT", Type = PermissionType.Grant, SecurableType = SecurableType.Database, SecurableName = masterDb.Name, DatabaseId = masterDb.Id, DatabaseRoleId = publicRole.Id, GrantedBy = "System", GrantedDate = DateTime.UtcNow, CreatedBy = "System", CreatedDate = DateTime.UtcNow },
                // Add more specific permissions as per SQL Server's default master DB permissions
            };
            foreach (var perm in masterDbPermissions)
            {
                // Simple check for uniqueness; real system permissions are complex to fully enumerate
                if (!await this.context.Permissions.AnyAsync(p => p.PermissionName == perm.PermissionName && p.SecurableName == perm.SecurableName && p.DatabaseId == perm.DatabaseId && p.DatabaseUserId == perm.DatabaseUserId && p.DatabaseRoleId == perm.DatabaseRoleId && p.ServerRoleId == perm.ServerRoleId))
                {
                    this.context.Permissions.Add(perm);
                }
            }
            await this.context.SaveChangesAsync();
        }

        Console.WriteLine("Seeding default SQL Server values completed.");
    }
}
