using Almakaber.BLL.DTOs.Admin;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Almakaber.BLL.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Grave> _graveRepo;
        private readonly IGenericRepository<Deceased> _deceasedRepo;
        private readonly IGenericRepository<Supplication> _supplicationRepo;
        private readonly IGenericRepository<DeceasedSupplication> _counterRepo;

        public DashboardService(
            UserManager<ApplicationUser> userManager,
            IGenericRepository<Grave> graveRepo,
            IGenericRepository<Deceased> deceasedRepo,
            IGenericRepository<Supplication> supplicationRepo,
            IGenericRepository<DeceasedSupplication> counterRepo)
        {
            _userManager = userManager;
            _graveRepo = graveRepo;
            _deceasedRepo = deceasedRepo;
            _supplicationRepo = supplicationRepo;
            _counterRepo = counterRepo;
        }

        public async Task<DashboardStatsDto> GetGlobalStatsAsync()
        {
            var totalUsers = await _userManager.Users.CountAsync(u => u.Email != "admin@almakaber.com");

            var totalGraves = (await _graveRepo.GetAllAsync()).Count();

            var totalDeceased = (await _deceasedRepo.GetAllAsync()).Count();

            var totalSupplications = (await _supplicationRepo.GetAllAsync()).Count();

            var allCounters = await _counterRepo.GetAllAsync();
            var totalPrayersCount = allCounters.Sum(c => c.Counter);

            return new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalGraves = totalGraves,
                TotalDeceased = totalDeceased,
                TotalSupplications = totalSupplications,
                TotalPrayersCount = totalPrayersCount
            };
        }
    }
}