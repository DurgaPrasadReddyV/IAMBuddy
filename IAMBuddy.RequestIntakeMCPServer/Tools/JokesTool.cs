using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMBuddy.RequestIntakeMCPServer.Tools
{
    [McpServerToolType]
    public class JokesTool
    {
        [McpServerTool, Description("Returns a joke about a specific topic")]
        public string GetJoke(string topic)
        {
            Console.WriteLine("==========================");
            Console.WriteLine($"Function get joke with topic: {topic}");
            var message = $"I don't do jokes about {topic}.";
            Console.WriteLine("Function report: " + message);
            Console.WriteLine($"Function End get joke with topic: {topic}");
            Console.WriteLine("==========================");
            return message;
        }
    }
}
