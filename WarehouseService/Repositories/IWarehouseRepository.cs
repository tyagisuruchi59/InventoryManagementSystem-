// StockPro Inventory Management System
// Service: Warehouse Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines database operations for Warehouse

using WarehouseService.Models;

namespace WarehouseService.Repositories
{
    public interface IWarehouseRepository
    {
        Task<List<Warehouse>> GetAllAsync();
        Task<Warehouse?> GetByIdAsync(int id);
        Task AddAsync(Warehouse warehouse);
        Task UpdateAsync(Warehouse warehouse);
        Task<List<StockLevel>> GetStockByWarehouseAsync(int warehouseId);
        Task<StockLevel?> GetStockLevelAsync(int warehouseId, int productId);
        Task AddStockLevelAsync(StockLevel stockLevel);
        Task UpdateStockLevelAsync(StockLevel stockLevel);
        Task<List<StockLevel>> GetLowStockAsync();
        Task<List<StockLevel>> GetOverStockAsync();
        Task AddTransferAsync(StockTransfer transfer);
        Task<List<StockTransfer>> GetTransferHistoryAsync();
        Task SaveChangesAsync();
    }
}