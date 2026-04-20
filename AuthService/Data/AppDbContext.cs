// StockPro Inventory Management System
// Service: Auth Service
// Data: AppDbContext
// Developer: Suru | April 2026
// Description: Connects to PostgreSQL database using Entity Framework Core

using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor — receives database options from Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Users table in PostgreSQL
        public DbSet<User> Users { get; set; }
    }
}