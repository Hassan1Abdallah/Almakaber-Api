using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Graves;

namespace Almakaber.BLL.Services.Interfaces
{
    public interface IGraveService
    {
        Task<PagedResponse<GraveDto>> GetAllGravesAsync(int pageNumber, int pageSize);
        Task<GraveDto> GetGraveByIdAsync(int id);
        Task<GraveDto> AddGraveAsync(CreateGraveDto dto);
        Task<bool> UpdateGraveAsync(int id, CreateGraveDto dto);
        Task<bool> DeleteGraveAsync(int id);
    }
}