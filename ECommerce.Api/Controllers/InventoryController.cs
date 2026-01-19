using System.Runtime.CompilerServices;
using ECommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controller;

[ApiController]
[Route("inventory")]

public class InventoryController : ControllerBase
{
    private readonly InventoryService _service;

    public InventoryController(InventoryService service)
    {
        _service = service;
    }

    [HttpGet("{sku}")]
    public async Task<IActionResult> GetInventoryByID(string sku)
    {
        try
        {
            var result = await _service.GetInventoryAscy(sku);
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}