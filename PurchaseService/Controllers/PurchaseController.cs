// StockPro Inventory Management System
// Service: Purchase Service | Controller: Purchase
// Developer: Suru | April 2026
// Description: API endpoints for purchase order lifecycle management

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseService.DTOs;
using PurchaseService.Services;

namespace PurchaseService.Controllers
{
    [ApiController]
    [Route("api/purchase-orders")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        // GET /api/purchase-orders - Get all POs
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var pos = await _purchaseService.GetAllPOsAsync();
            return Ok(pos);
        }

        // GET /api/purchase-orders/{id} - Get PO by ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var po = await _purchaseService.GetPOByIdAsync(id);
            if (po == null) return NotFound("Purchase order not found");
            return Ok(po);
        }

        // GET /api/purchase-orders/supplier/{supplierId}
        [HttpGet("supplier/{supplierId}")]
        [Authorize]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var pos = await _purchaseService.GetPOsBySupplierAsync(supplierId);
            return Ok(pos);
        }

        // GET /api/purchase-orders/warehouse/{warehouseId}
        [HttpGet("warehouse/{warehouseId}")]
        [Authorize]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            var pos = await _purchaseService.GetPOsByWarehouseAsync(warehouseId);
            return Ok(pos);
        }

        // GET /api/purchase-orders/status/{status}
        [HttpGet("status/{status}")]
        [Authorize]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var pos = await _purchaseService.GetPOsByStatusAsync(status);
            return Ok(pos);
        }

        // GET /api/purchase-orders/createdby/{userId}
        [HttpGet("createdby/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByCreatedBy(int userId)
        {
            var pos = await _purchaseService.GetPOsByCreatedByAsync(userId);
            return Ok(pos);
        }

        // GET /api/purchase-orders/daterange?from=&to=
        [HttpGet("daterange")]
        [Authorize]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var pos = await _purchaseService.GetPOsByDateRangeAsync(from, to);
            return Ok(pos);
        }

        // POST /api/purchase-orders - Create new PO
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,OFFICER")]
        public async Task<IActionResult> Create([FromBody] CreatePODto dto)
        {
            var result = await _purchaseService.CreatePOAsync(dto);
            return Ok(result);
        }

        // PUT /api/purchase-orders/{id} - Update PO
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,MANAGER,OFFICER")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePODto dto)
        {
            var result = await _purchaseService.UpdatePOAsync(id, dto);
            if (!result) return BadRequest("Cannot update — PO not found or not in Draft status");
            return Ok("Purchase order updated successfully");
        }

        // PUT /api/purchase-orders/{id}/approve - Approve PO
        [HttpPut("{id}/approve")]
       [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _purchaseService.ApprovePOAsync(id);
            if (result != "Purchase order approved successfully")
                return BadRequest(result);
            return Ok(result);
        }

        // POST /api/purchase-orders/receive - Receive goods (GRN)
        [HttpPost("receive")]
        [Authorize(Roles = "ADMIN,MANAGER,STAFF")]
        public async Task<IActionResult> ReceiveGoods([FromBody] ReceiveGoodsDto dto)
        {
            var result = await _purchaseService.ReceiveGoodsAsync(dto);
            if (result != "Goods received successfully")
                return BadRequest(result);
            return Ok(result);
        }

        // PUT /api/purchase-orders/{id}/cancel - Cancel PO
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _purchaseService.CancelPOAsync(id);
            if (result != "Purchase order cancelled successfully")
                return BadRequest(result);
            return Ok(result);
        }
    }
}