// StockPro Inventory Management System
// Service: Report Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines database operations for reports

using ReportService.Models;

namespace ReportService.Repositories
{
    public interface IReportRepository
    {
        // Add new snapshot
        Task AddSnapshotAsync(InventorySnapshot snapshot);

        // Get snapshots by warehouse
        Task<List<InventorySnapshot>> GetByWarehouseAsync(int warehouseId);

        // Get snapshots by product
        Task<List<InventorySnapshot>> GetByProductAsync(int productId);

        // Get snapshots by date range
        Task<List<InventorySnapshot>> GetByDateRangeAsync(DateTime from, DateTime to);

        // Get all snapshots
        Task<List<InventorySnapshot>> GetAllAsync();

        // Get total stock value
        Task<decimal> GetTotalStockValueAsync();

        // Get stock value by warehouse
        Task<Dictionary<int, decimal>> GetStockValueByWarehouseAsync();

        // Save changes
        Task SaveChangesAsync();
    }
}