using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Graves;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using AutoMapper;

namespace Almakaber.BLL.Services.Implementations
{
    public class GraveService : IGraveService
    {
        private readonly IGenericRepository<Grave> _repository;
        private readonly IMapper _mapper;

        public GraveService(IGenericRepository<Grave> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResponse<GraveDto>> GetAllGravesAsync(int pageNumber, int pageSize)
        {
            var (data, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize);

            var mappedData = _mapper.Map<IEnumerable<GraveDto>>(data);

            return new PagedResponse<GraveDto>
            {
                Data = mappedData,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<GraveDto> GetGraveByIdAsync(int id)
        {
            var grave = await _repository.GetByIdAsync(id);
            if (grave == null) return null;

            return _mapper.Map<GraveDto>(grave);
        }

        public async Task<GraveDto> AddGraveAsync(CreateGraveDto dto)
        {
            // بنحول الـ DTO اللي جاي من الفرونت لـ Entity عشان الداتا بيز تفهمه
            var grave = _mapper.Map<Grave>(dto);

            await _repository.AddAsync(grave);
            await _repository.SaveChangesAsync(); // هنا الـ CreatedAt بتتظبط أوتوماتيك

            // بنرجع الـ DTO بعد ما خد Id من الداتا بيز
            return _mapper.Map<GraveDto>(grave);
        }

        public async Task<bool> UpdateGraveAsync(int id, CreateGraveDto dto)
        {
            var grave = await _repository.GetByIdAsync(id);
            if (grave == null) return false;

            // بنحدث البيانات
            grave.StreetNumber = dto.StreetNumber;
            grave.GraveNumber = dto.GraveNumber;
            grave.GenderType = dto.GenderType;

            _repository.Update(grave);
            await _repository.SaveChangesAsync(); // هنا الـ UpdatedAt بتتظبط أوتوماتيك

            return true;
        }

        public async Task<bool> DeleteGraveAsync(int id)
        {
            var grave = await _repository.GetByIdAsync(id);
            if (grave == null) return false;

            _repository.Delete(grave);
            await _repository.SaveChangesAsync(); // هنا الـ IsDeleted بتتظبط لـ True أوتوماتيك

            return true;
        }
    }
}