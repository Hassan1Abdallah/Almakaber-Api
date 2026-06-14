using Almakaber.BLL.DTOs.Admin;
using Almakaber.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almakaber.API.Controllers
{
    [Authorize(Roles = "Admin")] 
    [Route("api/admin/dashboard")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public AdminDashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardStatsDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardService.GetGlobalStatsAsync();
            return Ok(stats);
        }
    }
}