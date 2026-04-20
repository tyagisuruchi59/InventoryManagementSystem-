// StockPro Inventory Management System
// Service: Product Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Actual database operations using Entity Framework Core

using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all active products
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        // Get product by ID
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        // Get product by SKU
        public async Task<Product?> GetBySKUAsync(string sku)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        // Get product by Barcode for QR scanning
        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        // Search by name, category, brand, or SKU
        public async Task<List<Product>> SearchAsync(string keyword)
        {
            return await _context.Products
                .Where(p => p.IsActive && (
                    p.Name.Contains(keyword) ||
                    p.Category.Contains(keyword) ||
                    p.Brand.Contains(keyword) ||
                    p.SKU.Contains(keyword)))
                .ToListAsync();
        }

        // Get products by category
        public async Task<List<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.IsActive && p.Category == category)
                .ToListAsync();
        }

        // Add new product to database
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        // Update product in database
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
        }

        // Save all changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}