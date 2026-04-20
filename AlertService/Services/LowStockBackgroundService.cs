// StockPro Inventory Management System
// Service: Alert Service | Background: LowStockChecker
// Developer: Suru | April 2026
// Description: IHostedService that runs every 15 minutes checking low stock
//              This is the UNIQUE feature - automatic alert generation

using AlertService.Data;
using Microsoft.EntityFrameworkCore;

namespace AlertService.Services
{
    public class LowStockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LowStockBackgroundService> _logger;

        // Run every 15 minutes
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

        public LowStockBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<LowStockBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Low Stock Background Service started");

            // Keep running until application stops
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckLowStockAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking low stock");
                }

                // Wait 15 minutes before next check
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CheckLowStockAsync()
        {
            // Create a new scope for database access
            using var scope = _serviceProvider.CreateScope();
            var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

            _logger.LogInformation("Checking low stock at {time}", DateTime.UtcNow);

            // In real system: call Warehouse Service API to get low stock items
            // For now: log that check ran successfully
            _logger.LogInformation("Low stock check completed at {time}", DateTime.UtcNow);

            // Example: trigger alert for demonstration
            await alertService.SendLowStockAlertAsync(
                productId: 0,
                warehouseId: 0,
                quantity: 0
            );
        }
    }
}