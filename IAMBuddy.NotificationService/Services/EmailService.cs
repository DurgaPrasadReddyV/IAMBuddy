using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IAMBuddy.NotificationService.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendApprovalRequestAsync(string toEmail, string subject, string approvalId, string description, string requestorEmail)
        {
            var baseUrl = "https://localhost:7001";
            var approveUrl = $"{baseUrl}/api/approval/approve/{approvalId}";
            var rejectUrl = $"{baseUrl}/api/approval/reject/{approvalId}";

            var body = $@"
                <html>
                <body>
                    <h2>Approval Request</h2>
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Requested by:</strong> {requestorEmail}</p>
                    <p><strong>Description:</strong></p>
                    <p>{description}</p>
                    <hr>
                    <p>Please click one of the buttons below to respond:</p>
                    <div style='margin: 20px 0;'>
                        <a href='{approveUrl}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; margin-right: 10px;'>APPROVE</a>
                        <a href='{rejectUrl}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>REJECT</a>
                    </div>
                    <p><small>This request will expire in 3 days if no response is received.</small></p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, $"Approval Required: {subject}", body);
        }

        public async Task SendReminderAsync(string toEmail, string subject, string approvalId, string description, int reminderNumber)
        {
            var baseUrl = "https://localhost:7001";
            var approveUrl = $"{baseUrl}/api/approval/approve/{approvalId}";
            var rejectUrl = $"{baseUrl}/api/approval/reject/{approvalId}";

            var body = $@"
                <html>
                <body>
                    <h2>Reminder #{reminderNumber}: Approval Request Pending</h2>
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Description:</strong></p>
                    <p>{description}</p>
                    <hr>
                    <p><strong>This is a reminder that your approval is still needed.</strong></p>
                    <div style='margin: 20px 0;'>
                        <a href='{approveUrl}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; margin-right: 10px;'>APPROVE</a>
                        <a href='{rejectUrl}' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>REJECT</a>
                    </div>
                    <p><small>This request will auto-reject if no response is received soon.</small></p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, $"REMINDER: Approval Required - {subject}", body);
        }

        public async Task SendApprovalNotificationAsync(string toEmail, string subject, bool isApproved, string? comment)
        {
            var status = isApproved ? "APPROVED" : "REJECTED";
            var color = isApproved ? "#28a745" : "#dc3545";

            var body = $@"
                <html>
                <body>
                    <h2 style='color: {color};'>Request {status}</h2>
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Status:</strong> <span style='color: {color}; font-weight: bold;'>{status}</span></p>
                    {(string.IsNullOrEmpty(comment) ? "" : $"<p><strong>Comment:</strong> {comment}</p>")}
                    <hr>
                    <p>Your approval request has been processed.</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, $"Request {status}: {subject}", body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            await Task.CompletedTask;
            Console.WriteLine("toEmail: " + toEmail);
            Console.WriteLine("subject: " + subject);
            Console.WriteLine("body: " + body);
        }
    }
}
