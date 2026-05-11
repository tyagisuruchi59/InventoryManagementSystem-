// StockPro Inventory Management System
// Service: Warehouse Service | Publisher
// Developer: Suru | April 2026
// Description: Publishes LowStockEvent to RabbitMQ when stock drops below reorder level

using MassTransit;
using WarehouseService.Events;

namespace WarehouseService.Publishers
{
    public class LowStockPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<LowStockPublisher> _logger;

        public LowStockPublisher(IPublishEndpoint publishEndpoint, ILogger<LowStockPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishLowStockAsync(int productId, string productName, int currentQty, int reorderLevel, int warehouseId, string warehouseName)
        {
            var evt = new LowStockEvent
            {
                ProductId = productId,
                ProductName = productName,
                CurrentQuantity = currentQty,
                ReorderLevel = reorderLevel,
                WarehouseId = warehouseId,
                WarehouseName = warehouseName,
                TriggeredAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(evt);

            _logger.LogInformation(
                "LowStockEvent published — Product #{ProductId} Qty: {Qty} below reorder level {ReorderLevel}",
                productId, currentQty, reorderLevel);
        }
    }
}