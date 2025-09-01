using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD_Provisioning_Demo.Models;

namespace AD_Provisioning_Demo
{
    public static class PolicyMatrix
    {
        public static readonly List<PolicyRule> Rules = new()
        {
            new PolicyRule("UserAccountRequest",new()
                {
                    new PolicyApproverStep("Manager", "ManagerOf(UserAccount)"),
                    new PolicyApproverStep("PlatformOwner", "PlatformOwner()")
                }
            ),
            new PolicyRule("ServiceAccountRequest",new()
                {
                    new PolicyApproverStep("ResourceOwner", "ResourceOwnerOf(ServiceAccount)"),
                    new PolicyApproverStep("PlatformOwner", "PlatformOwner()")
                }
            )
        };
    }
}
