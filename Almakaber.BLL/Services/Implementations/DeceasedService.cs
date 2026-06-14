using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Deceased;
using Almakaber.BLL.Helpers;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using AutoMapper;

namespace Almakaber.BLL.Services.Implementations
{
    public class DeceasedService : IDeceasedService
    {
        private readonly IGenericRepository<Deceased> _repository;
        private readonly IGenericRepository<Grave> _graveRepository;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public DeceasedService(IGenericRepository<Deceased> repository, IMapper mapper, IFileService fileService, IGenericRepository<Grave> graveRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _fileService = fileService;
            _graveRepository = graveRepository;
        }

        public async Task<PagedResponse<DeceasedDto>> GetAllDeceasedAsync(int pageNumber, int pageSize)
        {
            var (data, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize, d => d.Grave);

            var mappedData = _mapper.Map<IEnumerable<DeceasedDto>>(data);

            return new PagedResponse<DeceasedDto>
            {
                Data = mappedData,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<DeceasedDto> GetDeceasedByIdAsync(int id)
        {
            var deceased = await _repository.GetByIdAsync(id, d => d.Grave);
            if (deceased == null) return null;
            return _mapper.Map<DeceasedDto>(deceased);
        }

        public async Task<DeceasedDto> AddDeceasedAsync(CreateDeceasedDto dto)
        {
            var graveExists = await _graveRepository.GetByIdAsync(dto.GraveId);
            if (graveExists == null)
            {
                throw new ArgumentException("رقم المقبرة المحدد غير موجود في النظام.");
            }
            var deceased = _mapper.Map<Deceased>(dto);

            if (dto.Photo != null)
            {
                deceased.ImageUrl = await _fileService.SaveFileAsync(dto.Photo, "deceased");
            }

            await _repository.AddAsync(deceased);
            await _repository.SaveChangesAsync();
            return _mapper.Map<DeceasedDto>(deceased);
        }

        public async Task<bool> UpdateDeceasedAsync(int id, CreateDeceasedDto dto)
        {

            var graveExists = await _graveRepository.GetByIdAsync(dto.GraveId);
            if (graveExists == null)
            {
                throw new ArgumentException("رقم المقبرة المحدد غير موجود في النظام.");
            }

            var deceased = await _repository.GetByIdAsync(id);
            if (deceased == null) return false;

            deceased.FullName = dto.FullName;
            deceased.DateOfDeath = dto.DateOfDeath;
            deceased.GraveId = dto.GraveId;
            deceased.ImageUrl = await _fileService.SaveFileAsync(dto.Photo, "deceased");

            _repository.Update(deceased);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDeceasedAsync(int id)
        {
            var deceased = await _repository.GetByIdAsync(id);
            if (deceased == null) return false;

            _repository.Delete(deceased);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}