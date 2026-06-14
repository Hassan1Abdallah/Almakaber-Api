using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Supplications;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using AutoMapper;

namespace Almakaber.BLL.Services.Implementations
{
    public class SupplicationService : ISupplicationService
    {
        private readonly IGenericRepository<Supplication> _repository;
        private readonly IMapper _mapper;

        public SupplicationService(IGenericRepository<Supplication> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<SupplicationDto>> GetAllSupplicationsAsync(int pageNumber, int pageSize)
        {
            var (data, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize);
            var mappedData = _mapper.Map<IEnumerable<SupplicationDto>>(data);

            return new PagedResponse<SupplicationDto>
            {
                Data = mappedData,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<SupplicationDto> GetSupplicationByIdAsync(int id)
        {
            var supplication = await _repository.GetByIdAsync(id);
            if (supplication == null) return null;
            return _mapper.Map<SupplicationDto>(supplication);
        }

        public async Task<SupplicationDto> AddSupplicationAsync(CreateSupplicationDto dto)
        {
            var supplication = _mapper.Map<Supplication>(dto);
            await _repository.AddAsync(supplication);
            await _repository.SaveChangesAsync();
            return _mapper.Map<SupplicationDto>(supplication);
        }

        public async Task<bool> UpdateSupplicationAsync(int id, CreateSupplicationDto dto)
        {
            var supplication = await _repository.GetByIdAsync(id);
            if (supplication == null) return false;

            supplication.Title = dto.Title;
            supplication.Content = dto.Content;

            _repository.Update(supplication);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSupplicationAsync(int id)
        {
            var supplication = await _repository.GetByIdAsync(id);
            if (supplication == null) return false;

            _repository.Delete(supplication);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}