// StockPro Inventory Management System
// Service: Movement Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Implements all business logic for immutable stock movement audit trail.
// RecordMovement validates movement type before saving.
// GetStockIn/GetStockOut aggregate totals for analytics.
// All movements are write-once — no updates or deletions permitted.

using MovementService.DTOs;
using MovementService.Models;
using MovementService.Repositories;

namespace MovementService.Services
{
    public class MovementServiceImpl : IMovementService
    {
        private readonly IMovementRepository _repo;

        // Valid movement types as per documentation
        private readonly List<string> _validTypes = new()
        {
            "STOCK_IN", "STOCK_OUT", "TRANSFER_IN",
            "TRANSFER_OUT", "ADJUSTMENT", "WRITE_OFF", "RETURN"
        };

        public MovementServiceImpl(IMovementRepository repo)
        {
            _repo = repo;
        }

        // RECORD MOVEMENT — write once, no updates allowed
        public async Task<string> RecordMovementAsync(RecordMovementDto dto)
        {
            // Validate movement type
            if (!_validTypes.Contains(dto.MovementType.ToUpper()))
                return $"Invalid movement type. Valid types: {string.Join(", ", _validTypes)}";

            // Validate quantity must be positive
            if (dto.Quantity <= 0)
                return "Quantity must be greater than zero";

            var movement = new StockMovement
            {
                ProductId = dto.ProductId,
                WarehouseId = dto.WarehouseId,
                MovementType = dto.MovementType.ToUpper(),
                Quantity = dto.Quantity,
                ReferenceId = dto.ReferenceId,
                ReferenceType = dto.ReferenceType,
                UnitCost = dto.UnitCost,
                PerformedBy = dto.PerformedBy,
                Notes = dto.Notes,
                MovementDate = DateTime.UtcNow,
                BalanceAfter = dto.BalanceAfter
            };

            await _repo.AddAsync(movement);
            await _repo.SaveChangesAsync();
            return "Movement recorded successfully";
        }

        // GET BY PRODUCT
        public async Task<List<MovementResponseDto>> GetByProductAsync(int productId)
        {
            var movements = await _repo.FindByProductIdAsync(productId);
            return movements.Select(MapToDto).ToList();
        }

        // GET BY WAREHOUSE
        public async Task<List<MovementResponseDto>> GetByWarehouseAsync(int warehouseId)
        {
            var movements = await _repo.FindByWarehouseIdAsync(warehouseId);
            return movements.Select(MapToDto).ToList();
        }

        // GET BY TYPE
        public async Task<List<MovementResponseDto>> GetByTypeAsync(string movementType)
        {
            var movements = await _repo.FindByMovementTypeAsync(movementType.ToUpper());
            return movements.Select(MapToDto).ToList();
        }

        // GET BY DATE RANGE
        public async Task<List<MovementResponseDto>> GetByDateRangeAsync(
            DateTime from, DateTime to)
        {
            var movements = await _repo.FindByMovementDateBetweenAsync(from, to);
            return movements.Select(MapToDto).ToList();
        }

        // GET BY REFERENCE — e.g. find all movements for PO-001
        public async Task<List<MovementResponseDto>> GetByReferenceAsync(string referenceId)
        {
            var movements = await _repo.FindByReferenceIdAsync(referenceId);
            return movements.Select(MapToDto).ToList();
        }

        // GET MOVEMENT HISTORY — for specific product in specific warehouse
        public async Task<List<MovementResponseDto>> GetMovementHistoryAsync(
            int productId, int warehouseId)
        {
            var movements = await _repo.FindByProductAndWarehouseAsync(productId, warehouseId);
            return movements.Select(MapToDto).ToList();
        }

        // GET STOCK IN SUMMARY — totals all inbound movements
        public async Task<MovementSummaryDto> GetStockInAsync(int productId, int warehouseId)
        {
            var movements = await _repo.FindByProductAndWarehouseAsync(productId, warehouseId);

            // Inbound movement types
            var inboundTypes = new[] { "STOCK_IN", "TRANSFER_IN", "RETURN" };
            var outboundTypes = new[] { "STOCK_OUT", "TRANSFER_OUT", "WRITE_OFF" };

            int totalIn = movements
                .Where(m => inboundTypes.Contains(m.MovementType))
                .Sum(m => m.Quantity);

            int totalOut = movements
                .Where(m => outboundTypes.Contains(m.MovementType))
                .Sum(m => m.Quantity);

            return new MovementSummaryDto
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                TotalStockIn = totalIn,
                TotalStockOut = totalOut,
                NetMovement = totalIn - totalOut
            };
        }

        // GET STOCK OUT SUMMARY — totals all outbound movements
        public async Task<MovementSummaryDto> GetStockOutAsync(int productId, int warehouseId)
        {
            // Same calculation — returns full summary
            return await GetStockInAsync(productId, warehouseId);
        }

        // GET ALL MOVEMENTS — full audit trail
        public async Task<List<MovementResponseDto>> GetAllMovementsAsync()
        {
            var movements = await _repo.GetAllAsync();
            return movements.Select(MapToDto).ToList();
        }

        // MAP StockMovement to MovementResponseDto
        private MovementResponseDto MapToDto(StockMovement m)
        {
            return new MovementResponseDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                WarehouseId = m.WarehouseId,
                MovementType = m.MovementType,
                Quantity = m.Quantity,
                ReferenceId = m.ReferenceId,
                ReferenceType = m.ReferenceType,
                UnitCost = m.UnitCost,
                PerformedBy = m.PerformedBy,
                Notes = m.Notes,
                MovementDate = m.MovementDate,
                BalanceAfter = m.BalanceAfter
            };
        }
    }
}