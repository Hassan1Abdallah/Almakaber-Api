namespace Almakaber.BLL.Helpers.Email
{
    public interface IEmailSender
    {
        Task<(bool Success, string Message)> SendEmailAsync(string toEmail, string subject, string htmlMessage);
        string GetOtpTemplate(string userName, string otpCode, string purpose);
        string GetNewDeceasedNotificationTemplate(string deceasedName, string graveInfo);
    }
}