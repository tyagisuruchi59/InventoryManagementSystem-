// StockPro Inventory Management System
// Service: Alert Service | Data: DbContext
// Developer: Suru | April 2026
// Description: Connects to PostgreSQL alertdb database

using Microsoft.EntityFrameworkCore;
using AlertService.Models;

namespace AlertService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Alerts table in PostgreSQL
        public DbSet<Alert> Alerts { get; set; }
    }
}