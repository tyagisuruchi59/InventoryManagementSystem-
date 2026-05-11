using MassTransit;
using PurchaseService.Events;

namespace PurchaseService.Publishers
{
    public class POSubmittedPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<POSubmittedPublisher> _logger;

        public POSubmittedPublisher(IPublishEndpoint publishEndpoint, ILogger<POSubmittedPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishPOSubmittedAsync(int poId, string referenceNumber, int supplierId, int warehouseId, decimal totalAmount, string submittedBy)
        {
            var evt = new POSubmittedEvent
            {
                POId = poId,
                ReferenceNumber = referenceNumber,
                SupplierId = supplierId,
                WarehouseId = warehouseId,
                TotalAmount = totalAmount,
                SubmittedBy = submittedBy,
                SubmittedAt = DateTime.UtcNow
            };
            await _publishEndpoint.Publish(evt);
            _logger.LogInformation("POSubmittedEvent published for PO #{POId}", poId);
        }
    }
}
