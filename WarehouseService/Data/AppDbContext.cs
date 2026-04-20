// StockPro Inventory Management System
// Service: Warehouse Service | Data: DbContext
// Developer: Suru | April 2026
// Description: Connects to PostgreSQL warehousedb database

using Microsoft.EntityFrameworkCore;
using WarehouseService.Models;

namespace WarehouseService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Warehouses table
        public DbSet<Warehouse> Warehouses { get; set; }

        // Stock levels table - tracks stock per product per warehouse
        public DbSet<StockLevel> StockLevels { get; set; }

        // Stock transfers table - audit trail of transfers
        public DbSet<StockTransfer> StockTransfers { get; set; }
    }
}