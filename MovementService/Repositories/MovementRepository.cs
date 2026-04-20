// StockPro Inventory Management System
// Service: Movement Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Implements all database operations using EF Core.
// No Update or Delete methods — movements are immutable after creation.
// All queries ordered by MovementDate descending for audit trail display.

using Microsoft.EntityFrameworkCore;
using MovementService.Data;
using MovementService.Models;

namespace MovementService.Repositories
{
    public class MovementRepository : IMovementRepository
    {
        private readonly AppDbContext _context;

        public MovementRepository(AppDbContext context)
        {
            _context = context;
        }

        // Find all movements for a product
        public async Task<List<StockMovement>> FindByProductIdAsync(int productId)
            => await _context.StockMovements
                .Where(m => m.ProductId == productId)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Find all movements in a warehouse
        public async Task<List<StockMovement>> FindByWarehouseIdAsync(int warehouseId)
            => await _context.StockMovements
                .Where(m => m.WarehouseId == warehouseId)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Find movements by type e.g. STOCK_IN, WRITE_OFF
        public async Task<List<StockMovement>> FindByMovementTypeAsync(string movementType)
            => await _context.StockMovements
                .Where(m => m.MovementType == movementType)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Find movements linked to a specific reference e.g. PO ID
        public async Task<List<StockMovement>> FindByReferenceIdAsync(string referenceId)
            => await _context.StockMovements
                .Where(m => m.ReferenceId == referenceId)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Find movements within a date range — for reports
        public async Task<List<StockMovement>> FindByMovementDateBetweenAsync(
            DateTime from, DateTime to)
            => await _context.StockMovements
                .Where(m => m.MovementDate >= from && m.MovementDate <= to)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Find movements by performer — for user audit trail
        public async Task<List<StockMovement>> FindByPerformedByAsync(string performedBy)
            => await _context.StockMovements
                .Where(m => m.PerformedBy == performedBy)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Count movements by product and type — for analytics
        public async Task<int> CountByProductIdAndTypeAsync(int productId, string movementType)
            => await _context.StockMovements
                .CountAsync(m => m.ProductId == productId && m.MovementType == movementType);

        // Find movements for specific product in specific warehouse
        public async Task<List<StockMovement>> FindByProductAndWarehouseAsync(
            int productId, int warehouseId)
            => await _context.StockMovements
                .Where(m => m.ProductId == productId && m.WarehouseId == warehouseId)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Get all movements — full audit trail
        public async Task<List<StockMovement>> GetAllAsync()
            => await _context.StockMovements
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

        // Add new movement — no update/delete as movements are immutable
        public async Task AddAsync(StockMovement movement)
            => await _context.StockMovements.AddAsync(movement);

        // Save to database
        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}