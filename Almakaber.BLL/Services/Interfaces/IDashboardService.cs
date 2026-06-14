using Almakaber.BLL.DTOs.Admin;

namespace Almakaber.BLL.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetGlobalStatsAsync();
    }
}