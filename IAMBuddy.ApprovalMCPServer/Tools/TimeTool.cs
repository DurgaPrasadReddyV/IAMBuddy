using ModelContextProtocol.Server;
using System.ComponentModel;

namespace IAMBuddy.ApprovalMCPServer.Tools
{
    [McpServerToolType]
    public class TimeTool
    {

        [McpServerTool, Description("Get the current date and time")]
        public static DateTime Time()
        {
            return DateTime.UtcNow;
        }

        [McpServerTool, Description("get the current day of the week")]
        public static System.DayOfWeek DayOfWeek()
        {
            return DateTime.UtcNow.DayOfWeek;
        }
    }
}
