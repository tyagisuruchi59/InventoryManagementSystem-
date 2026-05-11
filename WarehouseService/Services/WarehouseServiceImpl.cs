// StockPro Inventory Management System
// Service: Warehouse Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Business logic for warehouse and stock management

using WarehouseService.DTOs;
using WarehouseService.Models;
using WarehouseService.Publishers;
using WarehouseService.Repositories;

namespace WarehouseService.Services
{
    public class WarehouseServiceImpl : IWarehouseService
    {
        private readonly IWarehouseRepository _repo;
        private readonly LowStockPublisher _lowStockPublisher;

        public WarehouseServiceImpl(IWarehouseRepository repo, LowStockPublisher lowStockPublisher)
        {
            _repo = repo;
            _lowStockPublisher = lowStockPublisher;
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync()
            => await _repo.GetAllAsync();

        public async Task<Warehouse?> GetWarehouseByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task<string> CreateWarehouseAsync(CreateWarehouseDto dto)
        {
            var warehouse = new Warehouse
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Capacity = dto.Capacity,
                ManagerId = dto.ManagerId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(warehouse);
            await _repo.SaveChangesAsync();
            return "Warehouse created successfully";
        }

        public async Task<bool> UpdateWarehouseAsync(int id, CreateWarehouseDto dto)
        {
            var warehouse = await _repo.GetByIdAsync(id);
            if (warehouse == null) return false;

            warehouse.Name = dto.Name;
            warehouse.Address = dto.Address;
            warehouse.City = dto.City;
            warehouse.Capacity = dto.Capacity;
            warehouse.ManagerId = dto.ManagerId;

            await _repo.UpdateAsync(warehouse);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateWarehouseAsync(int id)
        {
            var warehouse = await _repo.GetByIdAsync(id);
            if (warehouse == null) return false;

            warehouse.IsActive = false;
            await _repo.UpdateAsync(warehouse);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<List<StockLevel>> GetStockByWarehouseAsync(int warehouseId)
            => await _repo.GetStockByWarehouseAsync(warehouseId);

        public async Task<string> AddOrUpdateStockAsync(StockLevelDto dto)
        {
            var existing = await _repo.GetStockLevelAsync(dto.WarehouseId, dto.ProductId);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                existing.ReservedQuantity = dto.ReservedQuantity;
                existing.ReorderLevel = dto.ReorderLevel;
                existing.MaxStockLevel = dto.MaxStockLevel;
                existing.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateStockLevelAsync(existing);

                // 🐇 RABBITMQ — Publish LowStockEvent if stock drops below reorder level
                if (existing.Quantity < existing.ReorderLevel)
                {
                    var warehouse = await _repo.GetByIdAsync(dto.WarehouseId);
                    await _lowStockPublisher.PublishLowStockAsync(
                        productId: dto.ProductId,
                        productName: $"Product #{dto.ProductId}",
                        currentQty: existing.Quantity,
                        reorderLevel: existing.ReorderLevel,
                        warehouseId: dto.WarehouseId,
                        warehouseName: warehouse?.Name ?? $"Warehouse #{dto.WarehouseId}"
                    );
                }
            }
            else
            {
                var stockLevel = new StockLevel
                {
                    WarehouseId = dto.WarehouseId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    ReservedQuantity = dto.ReservedQuantity,
                    ReorderLevel = dto.ReorderLevel,
                    MaxStockLevel = dto.MaxStockLevel,
                    UpdatedAt = DateTime.UtcNow
                };
                await _repo.AddStockLevelAsync(stockLevel);

                // 🐇 RABBITMQ — Publish LowStockEvent for new stock if already below reorder level
                if (dto.Quantity < dto.ReorderLevel)
                {
                    var warehouse = await _repo.GetByIdAsync(dto.WarehouseId);
                    await _lowStockPublisher.PublishLowStockAsync(
                        productId: dto.ProductId,
                        productName: $"Product #{dto.ProductId}",
                        currentQty: dto.Quantity,
                        reorderLevel: dto.ReorderLevel,
                        warehouseId: dto.WarehouseId,
                        warehouseName: warehouse?.Name ?? $"Warehouse #{dto.WarehouseId}"
                    );
                }
            }

            await _repo.SaveChangesAsync();
            return "Stock updated successfully";
        }

        public async Task<List<StockLevel>> GetLowStockAsync()
            => await _repo.GetLowStockAsync();

        public async Task<List<StockLevel>> GetOverStockAsync()
            => await _repo.GetOverStockAsync();

        public async Task<string> TransferStockAsync(TransferStockDto dto)
        {
            var fromStock = await _repo.GetStockLevelAsync(dto.FromWarehouseId, dto.ProductId);

            if (fromStock == null || fromStock.AvailableQuantity < dto.Quantity)
                return "Not enough stock in source warehouse";

            fromStock.Quantity -= dto.Quantity;
            fromStock.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateStockLevelAsync(fromStock);

            // 🐇 RABBITMQ — Publish LowStockEvent after transfer if source drops below reorder level
            if (fromStock.Quantity < fromStock.ReorderLevel)
            {
                var warehouse = await _repo.GetByIdAsync(dto.FromWarehouseId);
                await _lowStockPublisher.PublishLowStockAsync(
                    productId: dto.ProductId,
                    productName: $"Product #{dto.ProductId}",
                    currentQty: fromStock.Quantity,
                    reorderLevel: fromStock.ReorderLevel,
                    warehouseId: dto.FromWarehouseId,
                    warehouseName: warehouse?.Name ?? $"Warehouse #{dto.FromWarehouseId}"
                );
            }

            var toStock = await _repo.GetStockLevelAsync(dto.ToWarehouseId, dto.ProductId);
            if (toStock != null)
            {
                toStock.Quantity += dto.Quantity;
                toStock.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateStockLevelAsync(toStock);
            }
            else
            {
                await _repo.AddStockLevelAsync(new StockLevel
                {
                    WarehouseId = dto.ToWarehouseId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _repo.AddTransferAsync(new StockTransfer
            {
                ProductId = dto.ProductId,
                FromWarehouseId = dto.FromWarehouseId,
                ToWarehouseId = dto.ToWarehouseId,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                TransferredBy = dto.TransferredBy,
                TransferredAt = DateTime.UtcNow
            });

            await _repo.SaveChangesAsync();
            return "Stock transferred successfully";
        }

        public async Task<List<StockTransfer>> GetTransferHistoryAsync()
            => await _repo.GetTransferHistoryAsync();
    }
}