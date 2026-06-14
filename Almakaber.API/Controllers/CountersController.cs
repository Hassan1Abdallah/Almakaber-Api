using Almakaber.BLL.DTOs.Supplications;
using Almakaber.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Almakaber.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountersController : ControllerBase
    {
        private readonly IDeceasedSupplicationService _counterService;


        public CountersController(IDeceasedSupplicationService counterService)
        {
            _counterService = counterService;
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                return userIdClaim;
            }

            throw new UnauthorizedAccessException("غير مصرح لك بالوصول.");
        }

        // GET: api/counters/deceased/5
        [HttpGet("deceased/{deceasedId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DeceasedSupplicationDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCountersForDeceased(int deceasedId)
        {
            var userId = GetCurrentUserId(); 

            var counters = await _counterService.GetCountersForDeceasedAsync(deceasedId, userId);
            return Ok(counters);
        }

        // POST: api/counters/deceased/5/supplication/2/increment
        [HttpPost("deceased/{deceasedId}/supplication/{supplicationId}/increment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> IncrementCounter(int deceasedId, int supplicationId)
        {
            var userId = GetCurrentUserId(); 

            var success = await _counterService.IncrementCounterAsync(deceasedId, supplicationId, userId);

            if (!success)
                return BadRequest(new { message = "حدث خطأ أثناء تحديث العداد." });

            return Ok(new { message = "تم زيادة العداد بنجاح." });
        }

        [HttpPost("deceased/{deceasedId}/toggle-reminder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ToggleReminder(int deceasedId)
        {
            string userId = GetCurrentUserId();
            var success = await _counterService.ToggleAnnualReminderAsync(deceasedId, userId);

            if (!success) return BadRequest(new { message = "حدث خطأ أثناء تحديث الإعدادات." });
            return Ok(new { message = "تم تحديث إعدادات التذكير السنوي بنجاح." });
        }
    }
}