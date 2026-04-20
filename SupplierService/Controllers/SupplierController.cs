// StockPro Inventory Management System
// Service: Supplier Service | Controller: Supplier
// Developer: Suru | April 2026
// Description: Exposes REST API endpoints for supplier management.
// Endpoints: POST (create), GET (by id/city/country/search/all),
// PUT (update/deactivate/rating), DELETE — as per documentation.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierService.DTOs;
using SupplierService.Services;

namespace SupplierService.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET /api/suppliers - Get all suppliers
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        // GET /api/suppliers/{id} - Get supplier by ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null) return NotFound("Supplier not found");
            return Ok(supplier);
        }

        // GET /api/suppliers/search/{name} - Search by name
        [HttpGet("search/{name}")]
        [Authorize]
        public async Task<IActionResult> Search(string name)
        {
            var suppliers = await _supplierService.SearchSuppliersAsync(name);
            return Ok(suppliers);
        }

        // GET /api/suppliers/city/{city} - Get by city (geo-filter)
        [HttpGet("city/{city}")]
        [Authorize]
        public async Task<IActionResult> GetByCity(string city)
        {
            var suppliers = await _supplierService.GetByCityAsync(city);
            return Ok(suppliers);
        }

        // GET /api/suppliers/country/{country} - Get by country (geo-filter)
        [HttpGet("country/{country}")]
        [Authorize]
        public async Task<IActionResult> GetByCountry(string country)
        {
            var suppliers = await _supplierService.GetByCountryAsync(country);
            return Ok(suppliers);
        }

        // GET /api/suppliers/count/active - Get active supplier count
        [HttpGet("count/active")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetActiveCount()
        {
            var count = await _supplierService.GetActiveCountAsync();
            return Ok(new { ActiveSuppliers = count });
        }

        // POST /api/suppliers - Create new supplier (Admin, Manager only)
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var result = await _supplierService.CreateSupplierAsync(dto);
            if (result != "Supplier created successfully")
                return BadRequest(result);
            return Ok(result);
        }

        // PUT /api/suppliers/{id} - Update supplier (Admin, Manager only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            var result = await _supplierService.UpdateSupplierAsync(id, dto);
            if (!result) return NotFound("Supplier not found");
            return Ok("Supplier updated successfully");
        }

        // PUT /api/suppliers/{id}/deactivate - Deactivate supplier (Admin only)
        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _supplierService.DeactivateSupplierAsync(id);
            if (!result) return NotFound("Supplier not found");
            return Ok("Supplier deactivated successfully");
        }

        // PUT /api/suppliers/{id}/rating - Update supplier rating
        [HttpPut("{id}/rating")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] UpdateRatingDto dto)
        {
            var result = await _supplierService.UpdateRatingAsync(id, dto);
            if (!result) return BadRequest("Supplier not found or rating value invalid (must be 0-5)");
            return Ok("Supplier rating updated successfully");
        }

        // DELETE /api/suppliers/{id} - Hard delete supplier (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result) return NotFound("Supplier not found");
            return Ok("Supplier deleted successfully");
        }
    }
}