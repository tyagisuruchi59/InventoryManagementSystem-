// StockPro Inventory Management System
// Service: Purchase Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines database operations for Purchase Orders

using PurchaseService.Models;

namespace PurchaseService.Repositories
{
    public interface IPurchaseRepository
    {
        Task<List<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder?> GetByIdAsync(int id);
        Task<List<PurchaseOrder>> GetBySupplierAsync(int supplierId);
        Task<List<PurchaseOrder>> GetByWarehouseAsync(int warehouseId);
        Task<List<PurchaseOrder>> GetByStatusAsync(string status);
        Task<List<PurchaseOrder>> GetByCreatedByAsync(int userId);
        Task<List<PurchaseOrder>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task AddAsync(PurchaseOrder po);
        Task UpdateAsync(PurchaseOrder po);
        Task SaveChangesAsync();
    }
}