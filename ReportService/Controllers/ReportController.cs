// StockPro Inventory Management System
// Service: Report Service | Controller: Report
// Developer: Suru | April 2026
// Description: API endpoints for inventory reports and analytics

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportService.DTOs;
using ReportService.Services;

namespace ReportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // POST /api/report/snapshot - Take inventory snapshot
        [HttpPost("snapshot")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> TakeSnapshot([FromBody] SnapshotDto dto)
        {
            var result = await _reportService.TakeSnapshotAsync(dto);
            return Ok(result);
        }

        // GET /api/report/totalvalue - Get total stock value
        [HttpGet("totalvalue")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetTotalValue()
        {
            var value = await _reportService.GetTotalStockValueAsync();
            return Ok(new { TotalStockValue = value });
        }

        // GET /api/report/bywarehouse - Get stock value per warehouse
        [HttpGet("bywarehouse")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetByWarehouseValue()
        {
            var data = await _reportService.GetStockValueByWarehouseAsync();
            return Ok(data);
        }

        // GET /api/report/warehouse/{id} - Get snapshots for warehouse
        [HttpGet("warehouse/{warehouseId}")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            var data = await _reportService.GetByWarehouseAsync(warehouseId);
            return Ok(data);
        }

        // GET /api/report/product/{id} - Get snapshots for product
        [HttpGet("product/{productId}")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var data = await _reportService.GetByProductAsync(productId);
            return Ok(data);
        }

        // GET /api/report/daterange - Get snapshots by date range
        [HttpGet("daterange")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var data = await _reportService.GetByDateRangeAsync(from, to);
            return Ok(data);
        }

        // GET /api/report/lowstock - Low stock report
        [HttpGet("lowstock")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetLowStock()
        {
            var report = await _reportService.GetLowStockReportAsync();
            return Ok(report);
        }

        // GET /api/report/topmoving - Top moving products
        [HttpGet("topmoving")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetTopMoving()
        {
            var report = await _reportService.GetTopMovingProductsAsync();
            return Ok(report);
        }

        // GET /api/report/slowmoving - Slow moving products
        [HttpGet("slowmoving")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetSlowMoving()
        {
            var report = await _reportService.GetSlowMovingProductsAsync();
            return Ok(report);
        }

        // GET /api/report/deadstock - Dead stock report
        [HttpGet("deadstock")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetDeadStock()
        {
            var report = await _reportService.GetDeadStockAsync();
            return Ok(report);
        }

        // GET /api/report/generate - Generate full inventory report
        [HttpGet("generate")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GenerateReport()
        {
            var report = await _reportService.GenerateInventoryReportAsync();
            return Ok(report);
        }
    }
}