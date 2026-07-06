using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Deceased;
using Almakaber.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almakaber.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeceasedController : ControllerBase
    {
        private readonly IDeceasedService _deceasedService;

        public DeceasedController(IDeceasedService deceasedService)
        {
            _deceasedService = deceasedService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<DeceasedDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllDeceased([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchName = null, [FromQuery] string? sortField = null, [FromQuery] int sortOrder = -1)
        {
            var result = await _deceasedService.GetAllDeceasedAsync(pageNumber, pageSize, searchName, sortField, sortOrder);
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DeceasedDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDeceasedById(int id)
        {
            var deceased = await _deceasedService.GetDeceasedByIdAsync(id);
            if (deceased == null)
                return NotFound(new { message = "بيانات المتوفي غير موجودة." });

            return Ok(deceased);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DeceasedDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateDeceased([FromForm] CreateDeceasedDto dto) 
        {
            var createdDeceased = await _deceasedService.AddDeceasedAsync(dto);

            return CreatedAtAction(nameof(GetDeceasedById), new { id = createdDeceased.Id }, createdDeceased);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateDeceased(int id, [FromForm] CreateDeceasedDto dto)
        {
            var success = await _deceasedService.UpdateDeceasedAsync(id, dto);
            if (!success)
                return NotFound(new { message = "بيانات المتوفي غير موجودة." });

            return Ok(new { message = "تم تعديل بيانات المتوفي بنجاح." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteDeceased(int id)
        {
            var success = await _deceasedService.DeleteDeceasedAsync(id);
            if (!success)
                return NotFound(new { message = "بيانات المتوفي غير موجودة." });

            return Ok(new { message = "تم حذف بيانات المتوفي بنجاح." });
        }
    }
}