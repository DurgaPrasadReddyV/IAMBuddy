using A2A.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMBuddy.Orchestrator.API.Models
{
    public class AgentData
    {
        public AgentCard AgentCard { get; set; }
        public string AgentName { get; set; }
        public string Name { get; set; }
        public Uri? URL { get; set; }
    }
}
