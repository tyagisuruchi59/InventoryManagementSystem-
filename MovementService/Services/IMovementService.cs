// StockPro Inventory Management System
// Service: Movement Service | Service: Interface
// Developer: Suru | April 2026
// Description: Declares all business logic operations for stock movement management.
// Covers movement recording, retrieval by entity/type/date,
// history lookup, and stock-in/out aggregation as per documentation.

using MovementService.DTOs;

namespace MovementService.Services
{
    public interface IMovementService
    {
        // Record a new stock movement — write once
        Task<string> RecordMovementAsync(RecordMovementDto dto);

        // Get all movements for a product
        Task<List<MovementResponseDto>> GetByProductAsync(int productId);

        // Get all movements in a warehouse
        Task<List<MovementResponseDto>> GetByWarehouseAsync(int warehouseId);

        // Get movements by type e.g. STOCK_IN
        Task<List<MovementResponseDto>> GetByTypeAsync(string movementType);

        // Get movements within a date range
        Task<List<MovementResponseDto>> GetByDateRangeAsync(DateTime from, DateTime to);

        // Get movements linked to a reference e.g. PO ID
        Task<List<MovementResponseDto>> GetByReferenceAsync(string referenceId);

        // Get full movement history for product in warehouse
        Task<List<MovementResponseDto>> GetMovementHistoryAsync(int productId, int warehouseId);

        // Get stock in totals for a product in a warehouse
        Task<MovementSummaryDto> GetStockInAsync(int productId, int warehouseId);

        // Get stock out totals for a product in a warehouse
        Task<MovementSummaryDto> GetStockOutAsync(int productId, int warehouseId);

        // Get all movements — full audit trail
        Task<List<MovementResponseDto>> GetAllMovementsAsync();
    }
}