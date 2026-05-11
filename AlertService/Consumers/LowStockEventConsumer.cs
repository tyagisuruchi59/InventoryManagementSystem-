// StockPro Inventory Management System
// Service: Alert Service | Consumer
// Developer: Suru | April 2026
// Description: Listens for LowStockEvent from WarehouseService via RabbitMQ

using MassTransit;
using AlertService.Data;
using AlertService.Models;
using AlertService.Events;

namespace AlertService.Consumers
{
    public class LowStockEventConsumer : IConsumer<LowStockEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LowStockEventConsumer> _logger;

        public LowStockEventConsumer(AppDbContext context, ILogger<LowStockEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<LowStockEvent> context)
        {
            var evt = context.Message;

            _logger.LogInformation(
                "LowStockEvent received — Product #{ProductId} in Warehouse #{WarehouseId} — Qty: {Qty}",
                evt.ProductId, evt.WarehouseId, evt.CurrentQuantity);

            // Create alert in database
            var alert = new Alert
            {
                Title = $"Low Stock Alert — Product #{evt.ProductId}",
                Message = $"Product #{evt.ProductId} ({evt.ProductName}) in {evt.WarehouseName} has only {evt.CurrentQuantity} units left. Reorder level is {evt.ReorderLevel}.",
                Type = "LOW_STOCK",
                Severity = "WARNING",
                RelatedProductId = evt.ProductId,
                RelatedWarehouseId = evt.WarehouseId,
                IsRead = false,
                IsAcknowledged = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Low stock alert saved to database for Product #{ProductId}", evt.ProductId);
        }
    }
}