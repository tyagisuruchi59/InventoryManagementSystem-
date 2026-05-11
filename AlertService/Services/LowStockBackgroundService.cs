using System.Text.Json;

namespace AlertService.Services
{
    public class LowStockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LowStockBackgroundService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

        public LowStockBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<LowStockBackgroundService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _config = config;
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
                var client = _httpClientFactory.CreateClient();
                var token = _config["ServiceToken"] ?? "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync("http://warehouse-service/api/warehouse/stock/lowstock");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Could not reach Warehouse Service (status {status}) â€” skipping", response.StatusCode);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<StockItem>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (items == null || items.Count == 0)
                {
                    _logger.LogInformation("No low stock items found");
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                foreach (var item in items)
                {
                    await alertService.SendLowStockAlertAsync(item.ProductId, item.WarehouseId, item.Quantity);
                    _logger.LogInformation("LOW_STOCK alert sent: Product #{pid} Warehouse #{wid} has {qty} units", item.ProductId, item.WarehouseId, item.Quantity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch low stock data");
            }
            _logger.LogInformation("Low stock check completed at {time}", DateTime.UtcNow);
        }

        private class StockItem
        {
            public int ProductId { get; set; }
            public int WarehouseId { get; set; }
            public int Quantity { get; set; }
            public int ReorderLevel { get; set; }
        }
    }
}
