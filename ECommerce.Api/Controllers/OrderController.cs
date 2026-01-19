using ECommerce.Api.DTOs;
using ECommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controller
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service;
        public OrderController(OrderService service)
        {
            _service = service;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrder(CreateOrder request)
        {
            try
            {
                var order = await _service.CreateOrderAsync(request);

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> Paymnet(int id)
        {
            try
            {
                await _service.PayOrderAsync(id);
                return Ok("Payment success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _service.CancelOrderAsync(id);
                return Ok("Order cancelled");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}