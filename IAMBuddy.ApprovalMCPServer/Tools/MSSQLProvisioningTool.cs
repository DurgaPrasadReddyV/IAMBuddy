using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMBuddy.ApprovalMCPServer.Tools
{
    [McpServerToolType]
    public sealed class MSSQLProvisioningTool
    {
        [McpServerTool, Description("Echoes the message back to the client.")]
        public static string Echo(string message) => $"Hello from C#: {message}";

        [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
        public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());
    }
}
