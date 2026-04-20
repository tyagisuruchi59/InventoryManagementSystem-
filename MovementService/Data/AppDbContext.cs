// StockPro Inventory Management System
// Service: Movement Service | Data: AppDbContext
// Developer: Suru | April 2026
// Description: EF Core DbContext connecting to PostgreSQL movementdb.
// StockMovements table is write-once — no updates or deletes allowed.
// Corrections must be made via a new opposing movement record.

using Microsoft.EntityFrameworkCore;
using MovementService.Models;

namespace MovementService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Stock movements table — immutable audit trail
        public DbSet<StockMovement> StockMovements { get; set; }
    }
}