// StockPro Inventory Management System
// Service: Report Service | Service: Interface
// Developer: Suru | April 2026
// Description: Defines business logic for reports and analytics

using ReportService.DTOs;
using ReportService.Models;

namespace ReportService.Services
{
    public interface IReportService
    {
        Task<string> TakeSnapshotAsync(SnapshotDto dto);
        Task<decimal> GetTotalStockValueAsync();
        Task<Dictionary<int, decimal>> GetStockValueByWarehouseAsync();
        Task<List<InventorySnapshot>> GetByWarehouseAsync(int warehouseId);
        Task<List<InventorySnapshot>> GetByProductAsync(int productId);
        Task<List<InventorySnapshot>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<ReportResponseDto> GetLowStockReportAsync();
        Task<ReportResponseDto> GetTopMovingProductsAsync();
        Task<ReportResponseDto> GetSlowMovingProductsAsync();
        Task<ReportResponseDto> GetDeadStockAsync();
        Task<ReportResponseDto> GenerateInventoryReportAsync();
    }
}