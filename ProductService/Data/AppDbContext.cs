// StockPro Inventory Management System
// Service: Product Service | Data: DbContext
// Developer: Suru | April 2026
// Description: Connects to PostgreSQL productdb database

using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // This creates the Products table in PostgreSQL
        public DbSet<Product> Products { get; set; }
    }
}