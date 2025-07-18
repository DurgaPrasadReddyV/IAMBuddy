using ModelContextProtocol.Server;
using System.ComponentModel;

namespace IAMBuddy.ProvisioningMCPServer.Tools
{
    [McpServerToolType]
    public class ScientificMathTool
    {
        [McpServerTool, Description("Take the sine of a number")]
        public static double Sin(
            [Description("The number to take the sine of")] double number1
        )
        {
            return Math.Sin(number1);
        }

        [McpServerTool, Description("Take the cosine of a number")]
        public static double Cos(
            [Description("The number to take the cosine of")] double number1
        )
        {
            return Math.Cos(number1);
        }

        [McpServerTool, Description("Take the tangent of a number")]
        public static double Tan(
            [Description("The number to take the tangent of")] double number1
        )
        {
            return Math.Tan(number1);
        }

        [McpServerTool, Description("Take the arcsine of a number")]
        public static double Asin(
            [Description("The number to take the arcsine of")] double number1
        )
        {
            return Math.Asin(number1);
        }

        [McpServerTool, Description("Take the arccosine of a number")]
        public static double Acos(
            [Description("The number to take the arccosine of")] double number1
        )
        {
            return Math.Acos(number1);
        }

        [McpServerTool, Description("Take the arctangent of a number")]
        public static double Atan(
            [Description("The number to take the arctangent of")] double number1
        )
        {
            return Math.Atan(number1);
        }
    }
}
