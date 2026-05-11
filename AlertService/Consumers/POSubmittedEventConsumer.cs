using PurchaseService.Events;
using MassTransit;
using AlertService.Data;
using AlertService.Models;

namespace AlertService.Consumers
{
    public class POSubmittedEventConsumer : IConsumer<POSubmittedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<POSubmittedEventConsumer> _logger;

        public POSubmittedEventConsumer(AppDbContext context, ILogger<POSubmittedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<POSubmittedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("POSubmittedEvent received — PO #{POId}", evt.POId);

            var alert = new Alert
            {
                RecipientId = 1,
                Title = $"PO Pending Approval — PO #{evt.POId}",
                Message = $"Purchase Order #{evt.POId} (Ref: {evt.ReferenceNumber}) submitted for approval. Total: {evt.TotalAmount}. Supplier #{evt.SupplierId}.",
                Type = "PO_PENDING",
                Severity = "MEDIUM",
                RelatedWarehouseId = evt.WarehouseId,
                Channel = "IN_APP",
                IsRead = false,
                IsAcknowledged = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            _logger.LogInformation("PO_PENDING alert saved for PO #{POId}", evt.POId);
        }
    }
}
