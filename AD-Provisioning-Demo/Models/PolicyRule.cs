using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD_Provisioning_Demo.Models
{
    public class PolicyRule
    {
        public PolicyRule(string requestType, List<PolicyApproverStep> steps, List<PolicyRuleDetail> rules)
        {
            RequestType = requestType;
            Steps = steps;
            Rules = rules;
        }

        public string RequestType { get; set; } = string.Empty; // e.g., "UserAccount", "ServiceAccount", "Group", "GroupMembership", supports wildcard like "Membership:*"
        public List<PolicyApproverStep> Steps { get; set; } = new();
        public List<PolicyRuleDetail> Rules { get; set; }
    }

    public class PolicyRuleDetail
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
    }
}
