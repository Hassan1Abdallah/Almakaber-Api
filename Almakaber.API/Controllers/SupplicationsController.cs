using Almakaber.BLL.DTOs.Common;
using Almakaber.BLL.DTOs.Supplications;
using Almakaber.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almakaber.API.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class SupplicationsController : ControllerBase
    {
        private readonly ISupplicationService _supplicationService;

        public SupplicationsController(ISupplicationService supplicationService)
        {
            _supplicationService = supplicationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<SupplicationDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var pagedResult = await _supplicationService.GetAllSupplicationsAsync(pageNumber, pageSize);
            return Ok(pagedResult);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplicationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supplicationService.GetSupplicationByIdAsync(id);
            if (result == null) return NotFound(new { message = "الدعاء غير موجود." });
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SupplicationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateSupplicationDto dto)
        {
            var result = await _supplicationService.AddSupplicationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(int id, [FromBody] CreateSupplicationDto dto)
        {
            var success = await _supplicationService.UpdateSupplicationAsync(id, dto);
            if (!success) return NotFound(new { message = "الدعاء غير موجود." });
            return Ok(new { message = "تم تعديل الدعاء بنجاح." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _supplicationService.DeleteSupplicationAsync(id);
            if (!success) return NotFound(new { message = "الدعاء غير موجود." });
            return Ok(new { message = "تم حذف الدعاء بنجاح." });
        }
    }
}