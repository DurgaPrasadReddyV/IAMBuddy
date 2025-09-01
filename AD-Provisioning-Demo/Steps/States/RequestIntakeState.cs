using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD_Provisioning_Demo.Models;

namespace AD_Provisioning_Demo.Steps.States
{
    public class RequestIntakeState
    {
        public ERequestType RequestType { get; set; } = ERequestType.Unknown;
        public ServiceAccountRequest ServiceAccountRequest { get; set; } = new();
        public UserAccountRequest UserAccountRequest { get; set; } = new();
        public DirectoryGroupRequest DirectoryGroupRequest { get; set; } = new();
        public DirectoryGroupMembershipRequest DirectoryGroupMembershipRequest { get; set; } = new();
        public List<ChatMessageContent> Conversation { get; set; } = [];
    }
}
