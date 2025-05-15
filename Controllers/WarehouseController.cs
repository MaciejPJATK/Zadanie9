using Tutorial9.Services;

using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;
using Tutorial9.Model;

namespace Tutorial9.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] AddProductRequest request, CancellationToken cancellationToken)
        {
            var result = await _warehouseService.AddProductToWarehouseAsync(request, cancellationToken);
            return Ok(new { IdProductWarehouse = result });
        }
    }
}
