namespace AD_Provisioning_Demo;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AD_Provisioning_Demo.Models;

public static class PolicyMatrixLoader
{
    private static List<PolicyRule>? _rules;

    public static List<PolicyRule> Rules
    {
        get
        {
            _rules ??= LoadRulesFromYaml("../../../PolicyMatrix/PolicyMatrix.yml");
            return _rules;
        }
    }

    private static List<PolicyRule> LoadRulesFromYaml(string path)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using var reader = new StreamReader(path);
        var yamlObject = deserializer.Deserialize<PolicyMatrixYaml>(reader);

        return [.. yamlObject.PolicyRules
            .Select(r => new PolicyRule(
                r.RequestType,
                [.. r.Steps.Select(s => new PolicyApproverStep(s.Role, s.Selector))]
            ))];
    }

    private class PolicyMatrixYaml
    {
        public List<PolicyRuleYaml> PolicyRules { get; set; }
    }

    private class PolicyRuleYaml
    {
        public string RequestType { get; set; }
        public List<PolicyApproverStepYaml> Steps { get; set; }
    }

    private class PolicyApproverStepYaml
    {
        public string Role { get; set; }
        public string Selector { get; set; }
    }
}
