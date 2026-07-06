using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Graves;
using Almakaber.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almakaber.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GravesController : ControllerBase
    {
        private readonly IGraveService _graveService;

        public GravesController(IGraveService graveService)
        {
            _graveService = graveService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<GraveDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllGraves([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var pagedResult = await _graveService.GetAllGravesAsync(pageNumber, pageSize);
            return Ok(pagedResult);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GraveDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetGraveById(int id)
        {
            var grave = await _graveService.GetGraveByIdAsync(id);

            if (grave == null)
                return NotFound(new { message = "المقبرة غير موجودة." });

            return Ok(grave);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GraveDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateGrave([FromBody] CreateGraveDto dto)
        {
            var createdGrave = await _graveService.AddGraveAsync(dto);

            return CreatedAtAction(nameof(GetGraveById), new { id = createdGrave.Id }, createdGrave);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateGrave(int id, [FromBody] CreateGraveDto dto)
        {
            var success = await _graveService.UpdateGraveAsync(id, dto);

            if (!success)
                return NotFound(new { message = "المقبرة غير موجودة." });

            return Ok(new { message = "تم تعديل بيانات المقبرة بنجاح." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteGrave(int id)
        {
            var success = await _graveService.DeleteGraveAsync(id);

            if (!success)
                return NotFound(new { message = "المقبرة غير موجودة." });

            return Ok(new { message = "تم حذف المقبرة بنجاح." });
        }
    }
}