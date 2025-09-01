namespace AD_Provisioning_Demo;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AD_Provisioning_Demo.Models;

public static class PoliciesLoader
{
    private static List<PolicyRule>? _rules;
    private static string? _currentFilename;

    // Method to get rules with a dynamic filename and caching
    public static List<PolicyRule> GetRules(string filename)
    {
        if (_rules == null || _currentFilename != filename)
        {
            _rules = LoadRulesFromYaml($"../../../Policies/{filename}.yml");
            _currentFilename = filename;
        }
        return _rules;
    }

    // Method to clear the cache (useful when switching between different files)
    public static void ClearCache()
    {
        _rules = null;
        _currentFilename = null;
    }

    private static List<PolicyRule> LoadRulesFromYaml(string path)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using var reader = new StreamReader(path);
        var yamlObject = deserializer.Deserialize<PolicyYaml>(reader);

        return [.. yamlObject.PolicyRules
            .Select(r => new PolicyRule(
                r.RequestType,
                [.. r.Steps.Select(s => new PolicyApproverStep(s.Role, s.Selector))],
                r.Rules?.Select(rule => new PolicyRuleDetail
                {
                    Name = rule.Name,
                    Description = rule.Description,
                    Condition = rule.Condition
                }).ToList() ?? new List<PolicyRuleDetail>()
            ))];
    }

    private class PolicyYaml
    {
        public List<PolicyRuleYaml> PolicyRules { get; set; }
    }

    private class PolicyRuleYaml
    {
        public string RequestType { get; set; }
        public List<PolicyApproverStepYaml> Steps { get; set; }
        public List<PolicyRuleDetailYaml> Rules { get; set; }
    }

    private class PolicyApproverStepYaml
    {
        public string Role { get; set; }
        public string Selector { get; set; }
    }

    private class PolicyRuleDetailYaml
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
    }
}
