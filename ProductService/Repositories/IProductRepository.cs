// StockPro Inventory Management System
// Service: Product Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines database operations for Product

using ProductService.Models;

namespace ProductService.Repositories
{
    public interface IProductRepository
    {
        // Get all products
        Task<List<Product>> GetAllAsync();

        // Get single product by ID
        Task<Product?> GetByIdAsync(int id);

        // Get product by SKU
        Task<Product?> GetBySKUAsync(string sku);

        // Get product by Barcode
        Task<Product?> GetByBarcodeAsync(string barcode);

        // Search products by name, category, brand, SKU
        Task<List<Product>> SearchAsync(string keyword);

        // Get products by category
        Task<List<Product>> GetByCategoryAsync(string category);

        // Add new product
        Task AddAsync(Product product);

        // Update existing product
        Task UpdateAsync(Product product);

        // Save changes to database
        Task SaveChangesAsync();
    }
}