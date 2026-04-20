// StockPro Inventory Management System
// Service: Product Service | Controller: Product
// Developer: Suru | April 2026
// Description: API endpoints for product catalogue management

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Services;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET /api/product - Get all products
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET /api/product/{id} - Get product by ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }

        // GET /api/product/sku/{sku} - Get product by SKU
        [HttpGet("sku/{sku}")]
        [Authorize]
        public async Task<IActionResult> GetBySKU(string sku)
        {
            var product = await _productService.GetProductBySKUAsync(sku);
            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }

        // GET /api/product/barcode/{barcode} - Get product by barcode (QR scan)
        [HttpGet("barcode/{barcode}")]
        [Authorize]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var product = await _productService.GetProductByBarcodeAsync(barcode);
            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }

        // GET /api/product/search/{keyword} - Search products
        [HttpGet("search/{keyword}")]
        [Authorize]
        public async Task<IActionResult> Search(string keyword)
        {
            var products = await _productService.SearchProductsAsync(keyword);
            return Ok(products);
        }

        // GET /api/product/category/{category} - Get by category
        [HttpGet("category/{category}")]
        [Authorize]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        // POST /api/product - Create new product (Manager, Admin)
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            if (result != "Product created successfully")
                return BadRequest(result);
            return Ok(result);
        }

        // PUT /api/product/{id} - Update product (Manager, Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateProductAsync(id, dto);
            if (!result) return NotFound("Product not found");
            return Ok("Product updated successfully");
        }

        // PUT /api/product/{id}/deactivate - Deactivate product (Admin only)
        [HttpPut("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _productService.DeactivateProductAsync(id);
            if (!result) return NotFound("Product not found");
            return Ok("Product deactivated successfully");
        }
    }
}