// StockPro Inventory Management System
// Service: Purchase Service | Data: DbContext
// Developer: Suru | April 2026
// Description: Connects to PostgreSQL purchasedb database

using Microsoft.EntityFrameworkCore;
using PurchaseService.Models;

namespace PurchaseService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Purchase orders table
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        // Line items table
        public DbSet<POLineItem> POLineItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TotalCost is computed - ignore it in database
            modelBuilder.Entity<POLineItem>()
                .Ignore(p => p.TotalCost);
        }
    }
}