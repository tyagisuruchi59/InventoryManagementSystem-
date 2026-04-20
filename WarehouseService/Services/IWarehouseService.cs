// StockPro Inventory Management System
// Service: Warehouse Service | Service: Interface
// Developer: Suru | April 2026
// Description: Defines business logic for warehouse management

using WarehouseService.DTOs;
using WarehouseService.Models;

namespace WarehouseService.Services
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetAllWarehousesAsync();
        Task<Warehouse?> GetWarehouseByIdAsync(int id);
        Task<string> CreateWarehouseAsync(CreateWarehouseDto dto);
        Task<bool> UpdateWarehouseAsync(int id, CreateWarehouseDto dto);
        Task<bool> DeactivateWarehouseAsync(int id);
        Task<List<StockLevel>> GetStockByWarehouseAsync(int warehouseId);
        Task<string> AddOrUpdateStockAsync(StockLevelDto dto);
        Task<List<StockLevel>> GetLowStockAsync();
        Task<List<StockLevel>> GetOverStockAsync();
        Task<string> TransferStockAsync(TransferStockDto dto);
        Task<List<StockTransfer>> GetTransferHistoryAsync();
    }
}