// StockPro Inventory Management System
// Service: Purchase Service | Publisher
// Developer: Suru | April 2026
// Description: Publishes POApprovedEvent to RabbitMQ when a PO is approved

using MassTransit;
using PurchaseService.Events;

namespace PurchaseService.Publishers
{
    public class POApprovedPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<POApprovedPublisher> _logger;

        public POApprovedPublisher(IPublishEndpoint publishEndpoint, ILogger<POApprovedPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishPOApprovedAsync(int poId, string referenceNumber, int supplierId, int warehouseId, decimal totalAmount, string approvedBy)
        {
            var evt = new POApprovedEvent
            {
                POId = poId,
                ReferenceNumber = referenceNumber,
                SupplierId = supplierId,
                WarehouseId = warehouseId,
                TotalAmount = totalAmount,
                ApprovedBy = approvedBy,
                ApprovedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(evt);

            _logger.LogInformation(
                "POApprovedEvent published — PO #{POId} approved by {ApprovedBy}",
                poId, approvedBy);
        }
    }
}