// StockPro Inventory Management System
// Service: Report Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Database operations using Entity Framework Core

using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Models;

namespace ReportService.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddSnapshotAsync(InventorySnapshot snapshot)
            => await _context.InventorySnapshots.AddAsync(snapshot);

        public async Task<List<InventorySnapshot>> GetByWarehouseAsync(int warehouseId)
            => await _context.InventorySnapshots
                .Where(s => s.WarehouseId == warehouseId)
                .OrderByDescending(s => s.SnapshotDate)
                .ToListAsync();

        public async Task<List<InventorySnapshot>> GetByProductAsync(int productId)
            => await _context.InventorySnapshots
                .Where(s => s.ProductId == productId)
                .OrderByDescending(s => s.SnapshotDate)
                .ToListAsync();

        public async Task<List<InventorySnapshot>> GetByDateRangeAsync(DateTime from, DateTime to)
            => await _context.InventorySnapshots
                .Where(s => s.SnapshotDate >= from && s.SnapshotDate <= to)
                .OrderByDescending(s => s.SnapshotDate)
                .ToListAsync();

        public async Task<List<InventorySnapshot>> GetAllAsync()
            => await _context.InventorySnapshots
                .OrderByDescending(s => s.SnapshotDate)
                .ToListAsync();

        public async Task<decimal> GetTotalStockValueAsync()
            => await _context.InventorySnapshots
                .Where(s => s.SnapshotDate.Date == DateTime.UtcNow.Date)
                .SumAsync(s => s.StockValue);

        public async Task<Dictionary<int, decimal>> GetStockValueByWarehouseAsync()
            => await _context.InventorySnapshots
                .Where(s => s.SnapshotDate.Date == DateTime.UtcNow.Date)
                .GroupBy(s => s.WarehouseId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(s => s.StockValue));

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}