using ModelContextProtocol.Server;
using System.ComponentModel;

namespace IAMBuddy.ProvisioningMCPServer.Tools
{
    [McpServerToolType]
    public class MathTool
    {
  //      [McpServerTool, Description("Add two numbers")]
  //      public static double Add(
  //    [Description("The first number to add")] double number1,
  //    [Description("The second number to add")] double number2
  //)
  //      {
  //          return number1 + number2;
  //      }

        [McpServerTool, Description("Subtract one number from another number")]
        public static double Subtract(
            [Description("The first number to subtract from")] double number1,
            [Description("The second number to subtract away")] double number2
        )
        {
            return number1 - number2;
        }

        [McpServerTool, Description("Multiply two numbers.")]
        public static double Multiply(
            [Description("The first number to multiply")] double number1,
            [Description("The second number to multiply")] double number2
        )
        {
            return number1 * number2;
        }

        [McpServerTool, Description("Divide one number by another number")]
        public static double Divide(
            [Description("The first number to divide from")] double number1,
            [Description("The second number to divide by")] double number2
        )
        {
            return number1 / number2;
        }

        [McpServerTool, Description("Raise one number to the power of another number")]
        public static double Power(
            [Description("The number to be raised to a power")] double number1,
            [Description("The power to raise the number by")] double number2
        )
        {
            return Math.Pow(number1, number2);
        }

        [McpServerTool, Description("Take the nth root of a number")]
        public static double Root(
           [Description("The number of which to take the root")] double number1,
           [Description("n")] double number2
        )
        {
            return Math.Pow(number1, 1.0 / number2);
        }

        [McpServerTool, Description("Take the log of a number")]
        public static double Log(
            [Description("The number to take the log of")] double number1,
            [Description("The base of the log")] double number2
        )
        {
            return Math.Log(number1, number2);
        }

        [McpServerTool, Description("Round a number to the target number of decimal places")]
        public static double Round(
            [Description("The number to round")] double number1,
            [Description("The number of decimal places to round to")] double number2
        )
        {
            return Math.Round(number1, (int)number2);
        }

        [McpServerTool, Description("Take the absolute value of a number")]
        public static double Abs(
            [Description("The number to take the absolute value of")] double number1
        )
        {
            return Math.Abs(number1);
        }

        [McpServerTool, Description("Take the floor of a number")]
        public static double Floor(
            [Description("The number to take the floor of")] double number1
        )
        {
            return Math.Floor(number1);
        }

        [McpServerTool, Description("Take the ceiling of a number")]
        public static double Ceiling(
            [Description("The number to take the ceiling of")] double number1
        )
        {
            return Math.Ceiling(number1);
        }

        
    }
}
