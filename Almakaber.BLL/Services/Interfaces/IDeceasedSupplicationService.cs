using Almakaber.BLL.DTOs.Supplications;

namespace Almakaber.BLL.Services.Interfaces
{
    public interface IDeceasedSupplicationService
    {
        Task<IEnumerable<DeceasedSupplicationDto>> GetCountersForDeceasedAsync(int deceasedId, string userId);
        Task<bool> ToggleAnnualReminderAsync(int deceasedId, string userId);
        Task<bool> IncrementCounterAsync(int deceasedId, int supplicationId, string userId);
    }
}