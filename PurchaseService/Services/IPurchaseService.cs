// StockPro Inventory Management System
// Service: Purchase Service | Service: Interface
// Developer: Suru | April 2026
// Description: Defines business logic for purchase order management

using PurchaseService.DTOs;

namespace PurchaseService.Services
{
    public interface IPurchaseService
    {
        Task<List<POResponseDto>> GetAllPOsAsync();
        Task<POResponseDto?> GetPOByIdAsync(int id);
        Task<List<POResponseDto>> GetPOsBySupplierAsync(int supplierId);
        Task<List<POResponseDto>> GetPOsByWarehouseAsync(int warehouseId);
        Task<List<POResponseDto>> GetPOsByStatusAsync(string status);
        Task<List<POResponseDto>> GetPOsByCreatedByAsync(int userId);
        Task<List<POResponseDto>> GetPOsByDateRangeAsync(DateTime from, DateTime to);
        Task<string> CreatePOAsync(CreatePODto dto);
        Task<bool> UpdatePOAsync(int id, UpdatePODto dto);
        Task<string> SubmitPOAsync(int id);
        Task<string> ApprovePOAsync(int id);
        Task<string> ReceiveGoodsAsync(ReceiveGoodsDto dto);
        Task<string> CancelPOAsync(int id);
    }
}