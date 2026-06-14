namespace Almakaber.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendFridayRemindersAsync();
        Task SendAnnualRemindersAsync();
    }
}