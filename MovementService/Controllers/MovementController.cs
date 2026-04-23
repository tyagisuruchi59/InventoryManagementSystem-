// StockPro Inventory Management System
// Service: Movement Service | Controller: Movement
// Developer: Suru | April 2026
// Description: Exposes REST API endpoints for stock movement audit trail.
// Endpoints: POST (record), GET (by product/warehouse/type/dateRange/
// reference/history), GET (stockIn/stockOut totals), GET all.
// All movements are immutable — no PUT or DELETE endpoints.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovementService.DTOs;
using MovementService.Services;

namespace MovementService.Controllers
{
    [ApiController]
    [Route("api/movements")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _movementService;

        public MovementController(IMovementService movementService)
        {
            _movementService = movementService;
        }

        // GET /api/movements - Get all movements (full audit trail)
        [HttpGet]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetAll()
        {
            var movements = await _movementService.GetAllMovementsAsync();
            return Ok(movements);
        }

        // GET /api/movements/product/{productId}
        [HttpGet("product/{productId}")]
        [Authorize]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var movements = await _movementService.GetByProductAsync(productId);
            return Ok(movements);
        }

        // GET /api/movements/warehouse/{warehouseId}
        [HttpGet("warehouse/{warehouseId}")]
        [Authorize]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            var movements = await _movementService.GetByWarehouseAsync(warehouseId);
            return Ok(movements);
        }

        // GET /api/movements/type/{movementType}
        [HttpGet("type/{movementType}")]
        [Authorize]
        public async Task<IActionResult> GetByType(string movementType)
        {
            var movements = await _movementService.GetByTypeAsync(movementType);
            return Ok(movements);
        }

        // GET /api/movements/daterange?from=&to=
        [HttpGet("daterange")]
        [Authorize]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var movements = await _movementService.GetByDateRangeAsync(from, to);
            return Ok(movements);
        }

        // GET /api/movements/reference/{referenceId}
        [HttpGet("reference/{referenceId}")]
        [Authorize]
        public async Task<IActionResult> GetByReference(string referenceId)
        {
            var movements = await _movementService.GetByReferenceAsync(referenceId);
            return Ok(movements);
        }

        // GET /api/movements/history/{productId}/{warehouseId}
        [HttpGet("history/{productId}/{warehouseId}")]
        [Authorize]
        public async Task<IActionResult> GetMovementHistory(int productId, int warehouseId)
        {
            var movements = await _movementService.GetMovementHistoryAsync(productId, warehouseId);
            return Ok(movements);
        }

        // GET /api/movements/stockin/{productId}/{warehouseId}
        [HttpGet("stockin/{productId}/{warehouseId}")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetStockIn(int productId, int warehouseId)
        {
            var summary = await _movementService.GetStockInAsync(productId, warehouseId);
            return Ok(summary);
        }

        // GET /api/movements/stockout/{productId}/{warehouseId}
        [HttpGet("stockout/{productId}/{warehouseId}")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetStockOut(int productId, int warehouseId)
        {
            var summary = await _movementService.GetStockOutAsync(productId, warehouseId);
            return Ok(summary);
        }

        // POST /api/movements - Record new movement (write once)
        [HttpPost]
        [Authorize(Roles = "ADMIN,MANAGER,STAFF")]
        public async Task<IActionResult> RecordMovement([FromBody] RecordMovementDto dto)
        {
            var result = await _movementService.RecordMovementAsync(dto);
            if (result != "Movement recorded successfully")
                return BadRequest(result);
            return Ok(result);
        }
    }
}