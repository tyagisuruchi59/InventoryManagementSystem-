// StockPro Inventory Management System
// Service: Report Service | Data: DbContext
// Developer: Suru | April 2026
// Description: Connects to PostgreSQL reportdb database

using Microsoft.EntityFrameworkCore;
using ReportService.Models;

namespace ReportService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Inventory snapshots table - written daily by background service
        public DbSet<InventorySnapshot> InventorySnapshots { get; set; }
    }
}