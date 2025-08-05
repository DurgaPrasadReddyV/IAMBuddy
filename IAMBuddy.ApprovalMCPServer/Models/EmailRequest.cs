using System.ComponentModel;

namespace IAMBuddy.ApprovalMCPServer.Models
{
    public class EmailRequest
    {
        [Description("List of email addresses to send the email to.")]
        public List<string> Receipents { get; set; }
        [Description("The template identifier or content for the email body.")]
        public string Template { get; set; }
        [Description("The subject line of the email.")]
        public string Subject { get; set; }
    }
}
