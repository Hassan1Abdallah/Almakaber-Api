using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Deceased;

namespace Almakaber.BLL.Services.Interfaces
{
    public interface IDeceasedService
    {
        Task<PagedResponse<DeceasedDto>> GetAllDeceasedAsync(int pageNumber, int pageSize);
        Task<DeceasedDto> GetDeceasedByIdAsync(int id);
        Task<DeceasedDto> AddDeceasedAsync(CreateDeceasedDto dto);
        Task<bool> UpdateDeceasedAsync(int id, CreateDeceasedDto dto);
        Task<bool> DeleteDeceasedAsync(int id);
    }
}