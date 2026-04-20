// StockPro Inventory Management System
// Service: Supplier Service | Data: AppDbContext
// Developer: Suru | April 2026
// Description: EF Core DbContext connecting to PostgreSQL supplierdb.
// Manages the Suppliers table with all supplier master data.

using Microsoft.EntityFrameworkCore;
using SupplierService.Models;

namespace SupplierService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Suppliers table in PostgreSQL
        public DbSet<Supplier> Suppliers { get; set; }
    }
}