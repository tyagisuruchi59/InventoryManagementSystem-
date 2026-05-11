// StockPro Inventory Management System
// Service: Alert Service | Service: Implementation
// Developer: Suru | April 2026

using AlertService.DTOs;
using AlertService.Models;
using AlertService.Repositories;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AlertService.Services
{
    public class AlertServiceImpl : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IConfiguration _configuration;

        public AlertServiceImpl(IAlertRepository alertRepository, IConfiguration configuration)
        {
            _alertRepository = alertRepository;
            _configuration = configuration;
        }

        public async Task<string> SendAlertAsync(CreateAlertDto dto)
        {
            var alert = new Alert
            {
                RecipientId = dto.RecipientId, Type = dto.Type, Severity = dto.Severity,
                Title = dto.Title, Message = dto.Message, RelatedProductId = dto.RelatedProductId,
                RelatedWarehouseId = dto.RelatedWarehouseId, Channel = dto.Channel,
                IsRead = false, IsAcknowledged = false, CreatedAt = DateTime.UtcNow
            };
            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
            return "Alert sent successfully";
        }

        public async Task<string> SendBulkAlertAsync(BulkAlertDto dto)
        {
            var alerts = dto.RecipientIds.Select(recipientId => new Alert
            {
                RecipientId = recipientId, Type = dto.Type, Severity = dto.Severity,
                Title = dto.Title, Message = dto.Message,
                IsRead = false, IsAcknowledged = false, CreatedAt = DateTime.UtcNow
            }).ToList();
            await _alertRepository.AddRangeAsync(alerts);
            await _alertRepository.SaveChangesAsync();
            return $"Bulk alert sent to {alerts.Count} recipients";
        }

        public async Task<string> SendLowStockAlertAsync(int productId, int warehouseId, int quantity)
        {
            var severity = quantity == 0 ? "CRITICAL" : "WARNING";
            var alert = new Alert
            {
                RecipientId = 1, Type = "LOW_STOCK", Severity = severity,
                Title = $"Low Stock Alert - Product #{productId}",
                Message = $"Product #{productId} in Warehouse #{warehouseId} has only {quantity} units remaining.",
                RelatedProductId = productId, RelatedWarehouseId = warehouseId,
                Channel = severity == "CRITICAL" ? "EMAIL" : "IN_APP",
                IsRead = false, IsAcknowledged = false, CreatedAt = DateTime.UtcNow
            };
            await _alertRepository.AddAsync(alert);
            await _alertRepository.SaveChangesAsync();
            if (severity == "CRITICAL")
            {
                var adminEmail = _configuration["SendGrid:FromEmail"];
                await SendEmailAsync(adminEmail, $"CRITICAL: Low Stock - Product #{productId}", alert.Message);
            }
            return "Low stock alert sent";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(_configuration["SendGrid:ApiKey"]);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = $"<div style='font-family:sans-serif;padding:24px;'><h2 style='color:#e94560;'>StockPro Alert</h2><p>{message}</p></div>"
            };
            msg.AddTo(new EmailAddress(toEmail));
            await client.SendEmailAsync(msg);
        }

        public async Task<List<Alert>> GetByRecipientAsync(int recipientId) => await _alertRepository.GetByRecipientAsync(recipientId);
        public async Task<List<Alert>> GetUnreadAsync(int recipientId) => await _alertRepository.GetUnreadByRecipientAsync(recipientId);
        public async Task<List<Alert>> GetUnacknowledgedAsync() => await _alertRepository.GetUnacknowledgedAsync();
        public async Task<List<Alert>> GetAllAsync() => await _alertRepository.GetAllAsync();
        public async Task<int> GetUnreadCountAsync(int recipientId) => await _alertRepository.CountUnreadAsync(recipientId);

        public async Task<bool> MarkAsReadAsync(int alertId)
        {
            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert == null) return false;
            alert.IsRead = true;
            await _alertRepository.UpdateAsync(alert);
            await _alertRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllReadAsync(int recipientId)
        {
            var alerts = await _alertRepository.GetUnreadByRecipientAsync(recipientId);
            foreach (var alert in alerts) alert.IsRead = true;
            await _alertRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AcknowledgeAsync(int alertId)
        {
            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert == null) return false;
            alert.IsAcknowledged = true;
            await _alertRepository.UpdateAsync(alert);
            await _alertRepository.SaveChangesAsync();
            return true;
        }

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
