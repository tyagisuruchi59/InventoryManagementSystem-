// StockPro Inventory Management System
// Service: Report Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Business logic for inventory analytics and reporting

using ReportService.DTOs;
using ReportService.Models;
using ReportService.Repositories;

namespace ReportService.Services
{
    public class ReportServiceImpl : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportServiceImpl(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        // TAKE SNAPSHOT - saves current stock state
        public async Task<string> TakeSnapshotAsync(SnapshotDto dto)
        {
            var snapshot = new InventorySnapshot
            {
                WarehouseId = dto.WarehouseId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                // Stock value = quantity x cost price
                StockValue = dto.Quantity * dto.CostPrice,
                SnapshotDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _reportRepository.AddSnapshotAsync(snapshot);
            await _reportRepository.SaveChangesAsync();
            return "Snapshot taken successfully";
        }

        // GET TOTAL STOCK VALUE across all warehouses
        public async Task<decimal> GetTotalStockValueAsync()
            => await _reportRepository.GetTotalStockValueAsync();

        // GET STOCK VALUE per warehouse
        public async Task<Dictionary<int, decimal>> GetStockValueByWarehouseAsync()
            => await _reportRepository.GetStockValueByWarehouseAsync();

        // GET SNAPSHOTS BY WAREHOUSE
        public async Task<List<InventorySnapshot>> GetByWarehouseAsync(int warehouseId)
            => await _reportRepository.GetByWarehouseAsync(warehouseId);

        // GET SNAPSHOTS BY PRODUCT
        public async Task<List<InventorySnapshot>> GetByProductAsync(int productId)
            => await _reportRepository.GetByProductAsync(productId);

        // GET SNAPSHOTS BY DATE RANGE
        public async Task<List<InventorySnapshot>> GetByDateRangeAsync(DateTime from, DateTime to)
            => await _reportRepository.GetByDateRangeAsync(from, to);

        // LOW STOCK REPORT - products below reorder level
        public async Task<ReportResponseDto> GetLowStockReportAsync()
        {
            var snapshots = await _reportRepository.GetAllAsync();
            var lowStock = snapshots
                .GroupBy(s => s.ProductId)
                .Where(g => g.OrderByDescending(s => s.SnapshotDate)
                    .First().Quantity < 10)
                .Select(g => new
                {
                    ProductId = g.Key,
                    CurrentQuantity = g.OrderByDescending(s => s.SnapshotDate).First().Quantity,
                    StockValue = g.OrderByDescending(s => s.SnapshotDate).First().StockValue
                })
                .ToList();

            return new ReportResponseDto
            {
                ReportType = "LOW_STOCK",
                GeneratedAt = DateTime.UtcNow,
                Data = lowStock
            };
        }

        // TOP MOVING PRODUCTS - highest stock value
        public async Task<ReportResponseDto> GetTopMovingProductsAsync()
        {
            var snapshots = await _reportRepository.GetAllAsync();
            var topMoving = snapshots
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalValue = g.Sum(s => s.StockValue),
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToList();

            return new ReportResponseDto
            {
                ReportType = "TOP_MOVING_PRODUCTS",
                GeneratedAt = DateTime.UtcNow,
                Data = topMoving
            };
        }

        // SLOW MOVING PRODUCTS - lowest movement
        public async Task<ReportResponseDto> GetSlowMovingProductsAsync()
        {
            var snapshots = await _reportRepository.GetAllAsync();
            var slowMoving = snapshots
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity),
                    TotalValue = g.Sum(s => s.StockValue)
                })
                .OrderBy(x => x.TotalQuantity)
                .Take(10)
                .ToList();

            return new ReportResponseDto
            {
                ReportType = "SLOW_MOVING_PRODUCTS",
                GeneratedAt = DateTime.UtcNow,
                Data = slowMoving
            };
        }

        // DEAD STOCK - no movement for 90 days
        public async Task<ReportResponseDto> GetDeadStockAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-90);
            var snapshots = await _reportRepository.GetAllAsync();

            var deadStock = snapshots
                .Where(s => s.SnapshotDate < cutoffDate)
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    LastSeen = g.Max(s => s.SnapshotDate),
                    StockValue = g.Sum(s => s.StockValue)
                })
                .ToList();

            return new ReportResponseDto
            {
                ReportType = "DEAD_STOCK",
                GeneratedAt = DateTime.UtcNow,
                Data = deadStock
            };
        }

        // GENERATE FULL INVENTORY REPORT
        public async Task<ReportResponseDto> GenerateInventoryReportAsync()
        {
            var totalValue = await _reportRepository.GetTotalStockValueAsync();
            var byWarehouse = await _reportRepository.GetStockValueByWarehouseAsync();
            var allSnapshots = await _reportRepository.GetAllAsync();

            return new ReportResponseDto
            {
                ReportType = "FULL_INVENTORY_REPORT",
                GeneratedAt = DateTime.UtcNow,
                Data = new
                {
                    TotalStockValue = totalValue,
                    StockValueByWarehouse = byWarehouse,
                    TotalSnapshots = allSnapshots.Count,
                    ReportDate = DateTime.UtcNow
                }
            };
        }
    }
}