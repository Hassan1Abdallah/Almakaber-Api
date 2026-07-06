using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Deceased;
using Almakaber.BLL.Helpers;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Context;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Almakaber.BLL.Services.Implementations
{
    public class DeceasedService : IDeceasedService
    {
        private readonly IGenericRepository<Deceased> _repository;
        private readonly IGenericRepository<Grave> _graveRepository;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly AlmakaberDbContext _context;

        public DeceasedService(IGenericRepository<Deceased> repository, IMapper mapper, IFileService fileService, IGenericRepository<Grave> graveRepository, AlmakaberDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _fileService = fileService;
            _graveRepository = graveRepository;
            _context = context;
        }

        public async Task<PagedResponse<DeceasedDto>> GetAllDeceasedAsync(int pageNumber, int pageSize, string? searchName = null, string? sortField = null, int sortOrder = -1)
        {
            var query = _context.DeceasedPersons.Include(d => d.Grave).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(d => d.FullName.Contains(searchName));
            }

            bool isAscending = sortOrder == 1;
            query = sortField switch
            {
                "fullName" => isAscending ? query.OrderBy(d => d.FullName) : query.OrderByDescending(d => d.FullName),
                "dateOfDeath" => isAscending ? query.OrderBy(d => d.DateOfDeath) : query.OrderByDescending(d => d.DateOfDeath),
                
                _ => isAscending ? query.OrderBy(d => d.Id) : query.OrderByDescending(d => d.Id)
            };

            var totalCount = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

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

            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                deceased.ImageUrl = await _fileService.SaveFileAsync(dto.Photo, "deceased");
            }
            else
            {
                deceased.ImageUrl = "/images/default-avatar.png"; 
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
            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                deceased.ImageUrl = await _fileService.SaveFileAsync(dto.Photo, "deceased");
            }
            else if (string.IsNullOrEmpty(deceased.ImageUrl))
            {
                deceased.ImageUrl = "/images/default-avatar.png";
            }   

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