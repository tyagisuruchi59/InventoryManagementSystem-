// StockPro Inventory Management System
// Service: Warehouse Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Database operations using Entity Framework Core

using Microsoft.EntityFrameworkCore;
using WarehouseService.Data;
using WarehouseService.Models;

namespace WarehouseService.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;

        public WarehouseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Warehouse>> GetAllAsync()
            => await _context.Warehouses.Where(w => w.IsActive).ToListAsync();

        public async Task<Warehouse?> GetByIdAsync(int id)
            => await _context.Warehouses.FindAsync(id);

        public async Task AddAsync(Warehouse warehouse)
            => await _context.Warehouses.AddAsync(warehouse);

        public async Task UpdateAsync(Warehouse warehouse)
            => _context.Warehouses.Update(warehouse);

        public async Task<List<StockLevel>> GetStockByWarehouseAsync(int warehouseId)
            => await _context.StockLevels
                .Where(s => s.WarehouseId == warehouseId)
                .ToListAsync();

        public async Task<StockLevel?> GetStockLevelAsync(int warehouseId, int productId)
            => await _context.StockLevels
                .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.ProductId == productId);

        public async Task AddStockLevelAsync(StockLevel stockLevel)
            => await _context.StockLevels.AddAsync(stockLevel);

        public async Task UpdateStockLevelAsync(StockLevel stockLevel)
            => _context.StockLevels.Update(stockLevel);

        public async Task<List<StockLevel>> GetLowStockAsync()
            => await _context.StockLevels
                .Where(s => s.Quantity < s.ReorderLevel)
                .ToListAsync();

        public async Task<List<StockLevel>> GetOverStockAsync()
            => await _context.StockLevels
                .Where(s => s.Quantity > s.MaxStockLevel)
                .ToListAsync();

        public async Task AddTransferAsync(StockTransfer transfer)
            => await _context.StockTransfers.AddAsync(transfer);

        public async Task<List<StockTransfer>> GetTransferHistoryAsync()
            => await _context.StockTransfers
                .OrderByDescending(t => t.TransferredAt)
                .ToListAsync();

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}