using ModelContextProtocol.Server;
using System.ComponentModel;

namespace IAMBuddy.RequestIntakeMCPServer.Tools
{
    [McpServerToolType]
    public class DayCheckTool
    {
        [McpServerTool, Description("Check if today is Monday and avoid math calculations")]
        public static string IsMonday()
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
            {
                return "Today is Monday. I don't want to do math calculations.";
            }
            else
            {
                return "Today is not Monday. Math calculations are allowed.";
            }
        }
    }
}
