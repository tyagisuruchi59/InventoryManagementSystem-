// StockPro Inventory Management System
// Service: Alert Service | Job: OverduePOAlertJob
// Developer: Suru | April 2026
// Description: Quartz.NET job — runs at 09:00 daily
//              Checks for approved POs past expected delivery date

using AlertService.DTOs;
using Quartz;

namespace AlertService.Services
{
    [DisallowConcurrentExecution]
    public class OverduePOAlertJob : IJob
    {
        private readonly ILogger<OverduePOAlertJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        public OverduePOAlertJob(
            ILogger<OverduePOAlertJob> logger,
            IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("OverduePOAlertJob started at {time}", DateTime.UtcNow);

            try
            {
                var client = _httpClientFactory.CreateClient();

                // Call PurchaseService using Docker service name
                var response = await client.GetAsync(
                    "http://purchase-service/api/purchase-orders/status/Approved");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Could not reach Purchase Service — skipping overdue PO check");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var pos = System.Text.Json.JsonSerializer.Deserialize<List<POItem>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (pos == null || pos.Count == 0)
                {
                    _logger.LogInformation("No approved POs found");
                    return;
                }

                // Find overdue POs (expected date passed but not received)
                var overduePOs = pos.Where(p => p.ExpectedDate < DateTime.UtcNow).ToList();

                if (overduePOs.Count == 0)
                {
                    _logger.LogInformation("No overdue POs found");
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                foreach (var po in overduePOs)
                {
                    var alertDto = new CreateAlertDto
                    {
                        RecipientId = 1, // ✅ FIXED: send to admin (was 0)
                        Title = $"Overdue PO — PO #{po.Id}",
                        Message = $"Purchase Order #{po.Id} (Ref: {po.ReferenceNumber}) was expected on {po.ExpectedDate:dd MMM yyyy} but has not been received.",
                        Type = "OVERDUE_RECEIPT",
                        Severity = "HIGH",
                        RelatedWarehouseId = po.WarehouseId,
                        Channel = "IN_APP"
                    };

                    await alertService.SendAlertAsync(alertDto);
                    _logger.LogInformation("OVERDUE_RECEIPT alert sent for PO #{poId}", po.Id);
                }

                _logger.LogInformation("OverduePOAlertJob completed — {count} alerts sent", overduePOs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OverduePOAlertJob failed");
            }
        }

        private class POItem
        {
            public int Id { get; set; }
            public string ReferenceNumber { get; set; } = string.Empty;
            public int WarehouseId { get; set; }
            public DateTime ExpectedDate { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}