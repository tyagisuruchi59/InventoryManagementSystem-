// StockPro Inventory Management System
// Service: Alert Service | Background: LowStockChecker
// Developer: Suru | April 2026
// Description: IHostedService that runs every 15 minutes checking low stock
//              Calls Warehouse Service API for real data — no duplicate spam

using System.Text.Json;

namespace AlertService.Services
{
    public class LowStockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LowStockBackgroundService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

        public LowStockBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<LowStockBackgroundService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Low Stock Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try { await CheckLowStockAsync(); }
                catch (Exception ex) { _logger.LogError(ex, "Error in low stock check"); }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CheckLowStockAsync()
        {
            _logger.LogInformation("Checking low stock at {time}", DateTime.UtcNow);

            try
            {
                // Call Warehouse Service to get real low stock items
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://localhost:5002/api/warehouse/stock/lowstock");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Could not reach Warehouse Service — skipping alert generation");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<StockItem>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (items == null || items.Count == 0)
                {
                    _logger.LogInformation("No low stock items found — no alerts needed");
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                // Send one alert per low stock item only
                foreach (var item in items)
                {
                    await alertService.SendLowStockAlertAsync(
                        productId: item.ProductId,
                        warehouseId: item.WarehouseId,
                        quantity: item.Quantity
                    );
                    _logger.LogInformation(
                        "Alert sent: Product #{pid} Warehouse #{wid} has {qty} units (below reorder level)",
                        item.ProductId, item.WarehouseId, item.Quantity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch low stock data from Warehouse Service");
            }

            _logger.LogInformation("Low stock check completed at {time}", DateTime.UtcNow);
        }

        // Matches Warehouse Service stock response
        private class StockItem
        {
            public int ProductId { get; set; }
            public int WarehouseId { get; set; }
            public int Quantity { get; set; }
            public int ReorderLevel { get; set; }
        }
    }
}