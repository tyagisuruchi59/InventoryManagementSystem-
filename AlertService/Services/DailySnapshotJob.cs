// StockPro Inventory Management System
// Service: Alert Service | Job: OverduePOAlertJob
// Developer: Suru | April 2026

using Quartz;

namespace AlertService.Services
{
    [DisallowConcurrentExecution]
    public class DailySnapshotJob : IJob
    {
        private readonly ILogger<DailySnapshotJob> _logger;

        public DailySnapshotJob(ILogger<DailySnapshotJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("DailySnapshotJob triggered at {time}", DateTime.UtcNow);
            await Task.CompletedTask;
        }
    }
}
