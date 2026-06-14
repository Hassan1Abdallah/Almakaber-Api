using Almakaber.BLL.Helpers.Email;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Almakaber.BLL.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Deceased> _deceasedRepo;
        private readonly IGenericRepository<DeceasedSupplication> _counterRepo;
        private readonly IEmailSender _emailSender;

        public NotificationService(
            UserManager<ApplicationUser> userManager,
            IGenericRepository<Deceased> deceasedRepo,
            IGenericRepository<DeceasedSupplication> counterRepo,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _deceasedRepo = deceasedRepo;
            _counterRepo = counterRepo;
            _emailSender = emailSender;
        }

        public async Task SendFridayRemindersAsync()
        {
            var users = await _userManager.Users.Where(u => u.EmailConfirmed).ToListAsync();

            string subject = "تذكير يوم الجمعة - منصة المقابر";
            string body = @"
                <div style='text-align: right; direction: rtl; font-family: Arial;'>
                    <h2>جمعة مباركة</h2>
                    <p>لا تنسوا الإكثار من الصلاة على النبي ﷺ في هذا اليوم المبارك.</p>
                    <p>ولا تنسوا أمواتكم وأموات المسلمين من صالح دعائكم.</p>
                </div>";

            foreach (var user in users)
            {
                await _emailSender.SendEmailAsync(user.Email, subject, body);
            }
        }

        public async Task SendAnnualRemindersAsync()
        {
            var today = DateTime.UtcNow;

            var deceasedList = await _deceasedRepo.FindAsync(d =>
                d.DateOfDeath.Month == today.Month &&
                d.DateOfDeath.Day == today.Day);

            foreach (var deceased in deceasedList)
            {
                var relatedCounters = await _counterRepo.FindAsync(c => c.DeceasedId == deceased.Id && c.WantsAnnualReminder == true);
                var userIds = relatedCounters.Select(c => c.UserId).Distinct().ToList();

                foreach (var userId in userIds)
                {
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user != null && user.EmailConfirmed)
                    {
                        string subject = $"الذكرى السنوية لوفاة - {deceased.FullName}";
                        string body = $@"
                            <div style='text-align: right; direction: rtl; font-family: Arial;'>
                                <h2>السلام عليكم {user.FullName}،</h2>
                                <p>يوافق اليوم الذكرى السنوية لوفاة المغفور له بإذن الله: <b>{deceased.FullName}</b>.</p>
                                <p>نسألكم ألا تنسوه من صالح دعائكم في هذا اليوم.</p>
                                <p><i>اللهم اغفر له وارحمه وعافه واعف عنه.</i></p>
                            </div>";

                        await _emailSender.SendEmailAsync(user.Email, subject, body);
                    }
                }
            }
        }
    }
}