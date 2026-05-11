// StockPro Inventory Management System
// Service: Report Service | Job: DailySnapshotJob
// Developer: Suru | April 2026
// Description: Quartz.NET job — runs at midnight daily

using Quartz;
using ReportService.DTOs;
namespace ReportService.Services
{
    [DisallowConcurrentExecution]
    public class DailySnapshotJob : IJob
    {
        private readonly ILogger<DailySnapshotJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DailySnapshotJob(
            ILogger<DailySnapshotJob> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("DailySnapshotJob started at {time}", DateTime.UtcNow);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
                await reportService.TakeSnapshotAsync(new SnapshotDto
{
    WarehouseId = 0,
    ProductId = 0,
    Quantity = 0,
    CostPrice = 0
});
                _logger.LogInformation("Daily snapshot completed at {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DailySnapshotJob failed");
            }
        }
    }
}