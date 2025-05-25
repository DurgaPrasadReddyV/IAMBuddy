namespace IAMBuddy.NotificationService.Services
{
    public interface IEmailService
    {
        Task SendApprovalRequestAsync(string toEmail, string subject, string approvalId, string description, string requestorEmail);
        Task SendReminderAsync(string toEmail, string subject, string approvalId, string description, int reminderNumber);
        Task SendApprovalNotificationAsync(string toEmail, string subject, bool isApproved, string? comment);
    }
}