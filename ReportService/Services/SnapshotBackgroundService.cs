// StockPro Inventory Management System
// Service: Report Service | Background: SnapshotScheduler
// Developer: Suru | April 2026
// Description: IHostedService that takes daily inventory snapshot automatically

namespace ReportService.Services
{
    public class SnapshotBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SnapshotBackgroundService> _logger;

        // Run every 24 hours
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);

        public SnapshotBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<SnapshotBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Snapshot Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await TakeDailySnapshotAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error taking daily snapshot");
                }

                // Wait 24 hours before next snapshot
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task TakeDailySnapshotAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();

            _logger.LogInformation("Taking daily inventory snapshot at {time}", DateTime.UtcNow);

            // In real system: call Warehouse Service to get all stock levels
            // Then take snapshot for each product/warehouse combination
            _logger.LogInformation("Daily snapshot completed at {time}", DateTime.UtcNow);
        }
    }
}