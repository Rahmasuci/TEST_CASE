using Healthcare.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly AvailabilitySlotService _service;

        public DoctorsController(AvailabilitySlotService service)
        {
            _service = service;
        }

        [HttpGet("{id}/availability")]
        public async Task<IActionResult> GetAvalibility(
            int id,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int slot
        )
        {
            try
            {
                if (slot % 5 != 0)
                    throw new Exception("Slot must be multiple of 5");

                if (slot != 15 && slot != 30 && slot != 60)
                    return BadRequest("Slot must be 15, 30, or 60 minutes");

                var result = await _service.GetAvailablityAsync(id, from, to, slot);

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}