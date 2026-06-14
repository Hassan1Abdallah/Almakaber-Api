using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Almakaber.BLL.Helpers.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message)> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "منصة المقابر والأدعية"),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }

                return (true, "تم إرسال البريد بنجاح");
            }
            catch (Exception ex)
            {
                return (false, $"خطأ في الإرسال: {ex.Message}");
            }
        }

        // --------------------------------------------------
        // القوالب الخاصة بمشروع المقابر (Templates)
        // --------------------------------------------------
        public string GetOtpTemplate(string userName, string otpCode, string purpose)
        {
            return $@"
                <div style='text-align: right; direction: rtl; font-family: Arial;'>
                    <h2>السلام عليكم {userName}،</h2>
                    <p>كود التحقق الخاص بك لـ (<b>{purpose}</b>) هو:</p>
                    <h1 style='color: #2c3e50; letter-spacing: 5px;'>{otpCode}</h1>
                    <p>هذا الكود صالح لمدة 5 دقائق فقط. لا تشاركه مع أحد.</p>
                    <p>نسأل الله لنا ولكم حسن الخاتمة.</p>
                </div>";
        }

        public string GetNewDeceasedNotificationTemplate(string deceasedName, string graveInfo)
        {
            return $@"
                <div style='text-align: right; direction: rtl; font-family: Arial;'>
                    <h2>إنا لله وإنا إليه راجعون</h2>
                    <p>انتقل إلى رحمة الله تعالى: <b>{deceasedName}</b></p>
                    <p>تم الدفن في: {graveInfo}</p>
                    <p>نسألكم الدعاء له بالرحمة والمغفرة، وأن يلهم أهله الصبر والسلوان.</p>
                    <p><i>اللهم اغفر له وارحمه وعافه واعف عنه.</i></p>
                </div>";
        }
    }
}