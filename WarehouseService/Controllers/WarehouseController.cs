// StockPro Inventory Management System
// Service: Warehouse Service | Controller: Warehouse
// Developer: Suru | April 2026
// Description: API endpoints for warehouse and stock management

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseService.DTOs;
using WarehouseService.Services;

namespace WarehouseService.Controllers
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

        // GET /api/warehouse - Get all warehouses
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        // GET /api/warehouse/{id} - Get warehouse by ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null) return NotFound("Warehouse not found");
            return Ok(warehouse);
        }

        // POST /api/warehouse - Create warehouse (Admin only)
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseDto dto)
        {
            var result = await _warehouseService.CreateWarehouseAsync(dto);
            return Ok(result);
        }

        // PUT /api/warehouse/{id} - Update warehouse (Admin only)
        [HttpPut("{id}")]
       [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateWarehouseDto dto)
        {
            var result = await _warehouseService.UpdateWarehouseAsync(id, dto);
            if (!result) return NotFound("Warehouse not found");
            return Ok("Warehouse updated successfully");
        }

        // PUT /api/warehouse/{id}/deactivate - Deactivate warehouse (Admin only)
        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _warehouseService.DeactivateWarehouseAsync(id);
            if (!result) return NotFound("Warehouse not found");
            return Ok("Warehouse deactivated successfully");
        }

        // GET /api/warehouse/{id}/stock - Get stock levels for warehouse
        [HttpGet("{id}/stock")]
        [Authorize]
        public async Task<IActionResult> GetStock(int id)
        {
            var stock = await _warehouseService.GetStockByWarehouseAsync(id);
            return Ok(stock);
        }

        // POST /api/warehouse/stock - Add or update stock level
        [HttpPost("stock")]
       [Authorize(Roles = "ADMIN,MANAGER,STAFF")]
        public async Task<IActionResult> AddOrUpdateStock([FromBody] StockLevelDto dto)
        {
            var result = await _warehouseService.AddOrUpdateStockAsync(dto);
            return Ok(result);
        }

        // GET /api/warehouse/stock/lowstock - Get all low stock items
        [HttpGet("stock/lowstock")]
        [AllowAnonymous]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetLowStock()
        {
            var stock = await _warehouseService.GetLowStockAsync();
            return Ok(stock);
        }

        // GET /api/warehouse/stock/overstock - Get all overstock items
        [HttpGet("stock/overstock")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetOverStock()
        {
            var stock = await _warehouseService.GetOverStockAsync();
            return Ok(stock);
        }

        // POST /api/warehouse/transfer - Transfer stock between warehouses
        [HttpPost("transfer")]
        [Authorize(Roles = "ADMIN,MANAGER,STAFF")]
        public async Task<IActionResult> Transfer([FromBody] TransferStockDto dto)
        {
            var result = await _warehouseService.TransferStockAsync(dto);
            if (result != "Stock transferred successfully")
                return BadRequest(result);
            return Ok(result);
        }

        // GET /api/warehouse/transfer/history - Get transfer history
        [HttpGet("transfer/history")]
        [Authorize]
        public async Task<IActionResult> GetTransferHistory()
        {
            var history = await _warehouseService.GetTransferHistoryAsync();
            return Ok(history);
        }
    }
}
