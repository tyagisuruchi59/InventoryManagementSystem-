// StockPro Inventory Management System
// Service: Product Service | Service: Interface
// Developer: Suru | April 2026
// Description: Defines business logic operations for products

using ProductService.DTOs;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto?> GetProductBySKUAsync(string sku);
        Task<ProductResponseDto?> GetProductByBarcodeAsync(string barcode);
        Task<List<ProductResponseDto>> SearchProductsAsync(string keyword);
        Task<List<ProductResponseDto>> GetProductsByCategoryAsync(string category);
        Task<string> CreateProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeactivateProductAsync(int id);
    }
}