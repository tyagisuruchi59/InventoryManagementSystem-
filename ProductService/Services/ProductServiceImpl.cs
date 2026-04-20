// StockPro Inventory Management System
// Service: Product Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Business logic for product catalogue management

using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductServiceImpl(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET ALL PRODUCTS
        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto).ToList();
        }

        // GET PRODUCT BY ID
        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        // GET PRODUCT BY SKU
        public async Task<ProductResponseDto?> GetProductBySKUAsync(string sku)
        {
            var product = await _productRepository.GetBySKUAsync(sku);
            return product == null ? null : MapToDto(product);
        }

        // GET PRODUCT BY BARCODE - used for QR scanning
        public async Task<ProductResponseDto?> GetProductByBarcodeAsync(string barcode)
        {
            var product = await _productRepository.GetByBarcodeAsync(barcode);
            return product == null ? null : MapToDto(product);
        }

        // SEARCH PRODUCTS
        public async Task<List<ProductResponseDto>> SearchProductsAsync(string keyword)
        {
            var products = await _productRepository.SearchAsync(keyword);
            return products.Select(MapToDto).ToList();
        }

        // GET BY CATEGORY
        public async Task<List<ProductResponseDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            return products.Select(MapToDto).ToList();
        }

        // CREATE PRODUCT
        public async Task<string> CreateProductAsync(CreateProductDto dto)
        {
            // Check if SKU already exists
            var existing = await _productRepository.GetBySKUAsync(dto.SKU);
            if (existing != null)
                return "Product with this SKU already exists";

            var product = new Product
            {
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Brand = dto.Brand,
                UnitOfMeasure = dto.UnitOfMeasure,
                CostPrice = dto.CostPrice,
                SellingPrice = dto.SellingPrice,
                Barcode = dto.Barcode,
                ReorderLevel = dto.ReorderLevel,
                MaxStockLevel = dto.MaxStockLevel,
                LeadTimeDays = dto.LeadTimeDays,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return "Product created successfully";
        }

        // UPDATE PRODUCT
        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Category = dto.Category;
            product.Brand = dto.Brand;
            product.UnitOfMeasure = dto.UnitOfMeasure;
            product.CostPrice = dto.CostPrice;
            product.SellingPrice = dto.SellingPrice;
            product.Barcode = dto.Barcode;
            product.ReorderLevel = dto.ReorderLevel;
            product.MaxStockLevel = dto.MaxStockLevel;
            product.LeadTimeDays = dto.LeadTimeDays;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }

        // DEACTIVATE PRODUCT - soft delete
        public async Task<bool> DeactivateProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }

        // MAP Product model to ProductResponseDto
        private ProductResponseDto MapToDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                SKU = product.SKU,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Brand = product.Brand,
                UnitOfMeasure = product.UnitOfMeasure,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                Barcode = product.Barcode,
                ReorderLevel = product.ReorderLevel,
                MaxStockLevel = product.MaxStockLevel,
                LeadTimeDays = product.LeadTimeDays,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }
    }
}