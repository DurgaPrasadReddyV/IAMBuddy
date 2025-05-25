using IAMBuddy.Shared.Models;
using System.Text.RegularExpressions;

namespace IAMBuddy.RequestIntakeService.Services
{
    public class RequestValidationService
    {
        private readonly ILogger<RequestValidationService> _logger;

        public RequestValidationService(ILogger<RequestValidationService> logger)
        {
            _logger = logger;
        }

        public async Task<ValidationResult> ValidateAccountRequestAsync(MSSQLAccountRequest request)
        {
            var result = new ValidationResult();

            // Basic required field validation
            ValidateRequiredFields(request, result);

            // Business rule validation
            ValidateBusinessRules(request, result);

            // Format validation
            ValidateFormats(request, result);

            _logger.LogInformation("Validation completed for request {RequestId}. Valid: {IsValid}, Errors: {ErrorCount}",
                request.Id, result.IsValid, result.Errors.Count);

            return await Task.FromResult(result);
        }

        private void ValidateRequiredFields(MSSQLAccountRequest request, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                result.Errors.Add("Username is required");

            if (string.IsNullOrWhiteSpace(request.DatabaseName))
                result.Errors.Add("Database name is required");

            if (string.IsNullOrWhiteSpace(request.ServerName))
                result.Errors.Add("Server name is required");

            if (string.IsNullOrWhiteSpace(request.RequestorEmail))
                result.Errors.Add("Requestor email is required");

            if (string.IsNullOrWhiteSpace(request.BusinessJustification))
                result.Errors.Add("Business justification is required");
        }

        private void ValidateBusinessRules(MSSQLAccountRequest request, ValidationResult result)
        {
            // Username length and format
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                if (request.Username.Length < 3 || request.Username.Length > 50)
                    result.Errors.Add("Username must be between 3 and 50 characters");

                if (!Regex.IsMatch(request.Username, @"^[a-zA-Z0-9_-]+$"))
                    result.Errors.Add("Username can only contain letters, numbers, underscores, and hyphens");
            }

            // Business justification minimum length
            if (!string.IsNullOrWhiteSpace(request.BusinessJustification) && request.BusinessJustification.Length < 10)
                result.Errors.Add("Business justification must be at least 10 characters");
        }

        private void ValidateFormats(MSSQLAccountRequest request, ValidationResult result)
        {
            // Email format validation
            if (!string.IsNullOrWhiteSpace(request.RequestorEmail))
            {
                var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(request.RequestorEmail, emailRegex))
                    result.Errors.Add("Invalid email format");
            }

            // Server name format
            if (!string.IsNullOrWhiteSpace(request.ServerName))
            {
                if (!Regex.IsMatch(request.ServerName, @"^[a-zA-Z0-9.-]+$"))
                    result.Errors.Add("Server name contains invalid characters");
            }

            // Database name format
            if (!string.IsNullOrWhiteSpace(request.DatabaseName))
            {
                if (!Regex.IsMatch(request.DatabaseName, @"^[a-zA-Z0-9_]+$"))
                    result.Errors.Add("Database name can only contain letters, numbers, and underscores");
            }
        }
    }

    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
