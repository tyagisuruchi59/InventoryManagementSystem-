using AlertService.Events;
using MassTransit;
using AlertService.Data;
using AlertService.Models;

namespace AlertService.Consumers
{
    public class POApprovedEventConsumer : IConsumer<POApprovedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<POApprovedEventConsumer> _logger;

        public POApprovedEventConsumer(AppDbContext context, ILogger<POApprovedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<POApprovedEvent> context)
        {
            var evt = context.Message;
            var alert = new Alert
            {
                RecipientId = 1,
                Title = "Purchase Order Approved - PO #" + evt.POId,
                Message = "PO #" + evt.POId + " has been approved by " + evt.ApprovedBy + ". Total: " + evt.TotalAmount,
                Type = "PO_APPROVED",
                Severity = "INFO",
                RelatedWarehouseId = evt.WarehouseId,
                Channel = "IN_APP",
                IsRead = false,
                IsAcknowledged = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
        }
    }
}