// StockPro Inventory Management System
// Service: Alert Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Business logic for alert management and notifications

using AlertService.DTOs;
using AlertService.Models;
using AlertService.Repositories;

namespace AlertService.Services
{
    public class AlertServiceImpl : IAlertService
    {
        private readonly IAlertRepository _alertRepository;

        public AlertServiceImpl(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        // SEND SINGLE ALERT
        public async Task<string> SendAlertAsync(CreateAlertDto dto)
        {
            var alert = new Alert
            {
                RecipientId = dto.RecipientId,
                Type = dto.Type,
                Severity = dto.Severity,
                Title = dto.Title,
                Message = dto.Message,
                RelatedProductId = dto.RelatedProductId,
                RelatedWarehouseId = dto.RelatedWarehouseId,
                Channel = dto.Channel,
                IsRead = false,
                IsAcknowledged = false,
                CreatedAt = DateTime.UtcNow
            };

            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
            return "Alert sent successfully";
        }

        // SEND BULK ALERT to multiple recipients
        public async Task<string> SendBulkAlertAsync(BulkAlertDto dto)
        {
            var alerts = dto.RecipientIds.Select(recipientId => new Alert
            {
                RecipientId = recipientId,
                Type = dto.Type,
                Severity = dto.Severity,
                Title = dto.Title,
                Message = dto.Message,
                IsRead = false,
                IsAcknowledged = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _alertRepository.AddRangeAsync(alerts);
            await _alertRepository.SaveChangesAsync();
            return $"Bulk alert sent to {alerts.Count} recipients";
        }

        // SEND LOW STOCK ALERT - called by IHostedService background job
        public async Task<string> SendLowStockAlertAsync(int productId, int warehouseId, int quantity)
        {
            var alert = new Alert
            {
                RecipientId = 1, // Send to admin by default
                Type = "LOW_STOCK",
                Severity = quantity == 0 ? "CRITICAL" : "WARNING",
                Title = $"Low Stock Alert - Product #{productId}",
                Message = $"Product #{productId} in Warehouse #{warehouseId} has only {quantity} units remaining. Please reorder immediately.",
                RelatedProductId = productId,
                RelatedWarehouseId = warehouseId,
                Channel = quantity == 0 ? "EMAIL" : "IN_APP",
                IsRead = false,
                IsAcknowledged = false,
                CreatedAt = DateTime.UtcNow
            };

            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
            return "Low stock alert sent";
        }

        public async Task<List<Alert>> GetByRecipientAsync(int recipientId)
            => await _alertRepository.GetByRecipientAsync(recipientId);

        public async Task<List<Alert>> GetUnreadAsync(int recipientId)
            => await _alertRepository.GetUnreadByRecipientAsync(recipientId);

        public async Task<List<Alert>> GetUnacknowledgedAsync()
            => await _alertRepository.GetUnacknowledgedAsync();

        public async Task<List<Alert>> GetAllAsync()
            => await _alertRepository.GetAllAsync();

        public async Task<int> GetUnreadCountAsync(int recipientId)
            => await _alertRepository.CountUnreadAsync(recipientId);

        // MARK ALERT AS READ
        public async Task<bool> MarkAsReadAsync(int alertId)
        {
            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert == null) return false;

            alert.IsRead = true;
            await _alertRepository.UpdateAsync(alert);
            await _alertRepository.SaveChangesAsync();
            return true;
        }

        // MARK ALL ALERTS AS READ for a recipient
        public async Task<bool> MarkAllReadAsync(int recipientId)
        {
            var alerts = await _alertRepository.GetUnreadByRecipientAsync(recipientId);
            foreach (var alert in alerts)
                alert.IsRead = true;

            await _alertRepository.SaveChangesAsync();
            return true;
        }

        // ACKNOWLEDGE ALERT
        public async Task<bool> AcknowledgeAsync(int alertId)
        {
            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert == null) return false;

            alert.IsAcknowledged = true;
            await _alertRepository.UpdateAsync(alert);
            await _alertRepository.SaveChangesAsync();
            return true;
        }

        // DELETE ALERT
        public async Task<bool> DeleteAlertAsync(int alertId)
        {
            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert == null) return false;

            await _alertRepository.DeleteAsync(alertId);
            await _alertRepository.SaveChangesAsync();
            return true;
        }
    }
}