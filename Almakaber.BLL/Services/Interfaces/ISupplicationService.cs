using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Supplications;

namespace Almakaber.BLL.Services.Interfaces
{
    public interface ISupplicationService
    {
        Task<PagedResponse<SupplicationDto>> GetAllSupplicationsAsync(int pageNumber, int pageSize);
        Task<SupplicationDto> GetSupplicationByIdAsync(int id);
        Task<SupplicationDto> AddSupplicationAsync(CreateSupplicationDto dto);
        Task<bool> UpdateSupplicationAsync(int id, CreateSupplicationDto dto);
        Task<bool> DeleteSupplicationAsync(int id);
    }
}