using Almakaber.BLL.DTOs.Account;
using Almakaber.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almakaber.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public UsersController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers(
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10,
                    [FromQuery] string? searchName = null,
                    [FromQuery] string? sortField = null,
                    [FromQuery] int sortOrder = 1)
            {
            var result = await _accountService.GetAllUsersAsync(pageNumber, pageSize, searchName, sortField, sortOrder);

            return Ok(new
            {
                data = result.Data,
                totalCount = result.TotalCount
            });
        }

        [HttpPost("{id}/toggle-block")]
        public async Task<IActionResult> ToggleBlockUser(string id)
        {
            var result = await _accountService.ToggleBlockUserAsync(id);
            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}