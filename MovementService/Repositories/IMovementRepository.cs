// StockPro Inventory Management System
// Service: Movement Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines all database operations for StockMovement entity.
// Follows Repository pattern as per documentation class diagram.
// Methods: FindByProductId, FindByWarehouseId, FindByMovementType,
// FindByReferenceId, FindByMovementDateBetween,
// FindByPerformedBy, CountByProductIdAndType.

using MovementService.Models;

namespace MovementService.Repositories
{
    public interface IMovementRepository
    {
        // Find movements by product ID
        Task<List<StockMovement>> FindByProductIdAsync(int productId);

        // Find movements by warehouse ID
        Task<List<StockMovement>> FindByWarehouseIdAsync(int warehouseId);

        // Find movements by movement type e.g. STOCK_IN
        Task<List<StockMovement>> FindByMovementTypeAsync(string movementType);

        // Find movements by reference ID e.g. PO-001
        Task<List<StockMovement>> FindByReferenceIdAsync(string referenceId);

        // Find movements within a date range
        Task<List<StockMovement>> FindByMovementDateBetweenAsync(DateTime from, DateTime to);

        // Find movements performed by a specific user
        Task<List<StockMovement>> FindByPerformedByAsync(string performedBy);

        // Count movements by product and type
        Task<int> CountByProductIdAndTypeAsync(int productId, string movementType);

        // Find movements by product and warehouse together
        Task<List<StockMovement>> FindByProductAndWarehouseAsync(int productId, int warehouseId);

        // Get all movements ordered by date descending
        Task<List<StockMovement>> GetAllAsync();

        // Add new movement — write once only
        Task AddAsync(StockMovement movement);

        // Save changes to database
        Task SaveChangesAsync();
    }
}