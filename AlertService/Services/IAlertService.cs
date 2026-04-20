// StockPro Inventory Management System
// Service: Alert Service | Service: Interface
// Developer: Suru | April 2026
// Description: Defines business logic operations for alerts

using AlertService.DTOs;
using AlertService.Models;

namespace AlertService.Services
{
    public interface IAlertService
    {
        Task<string> SendAlertAsync(CreateAlertDto dto);
        Task<string> SendBulkAlertAsync(BulkAlertDto dto);
        Task<string> SendLowStockAlertAsync(int productId, int warehouseId, int quantity);
        Task<List<Alert>> GetByRecipientAsync(int recipientId);
        Task<List<Alert>> GetUnreadAsync(int recipientId);
        Task<List<Alert>> GetUnacknowledgedAsync();
        Task<List<Alert>> GetAllAsync();
        Task<int> GetUnreadCountAsync(int recipientId);
        Task<bool> MarkAsReadAsync(int alertId);
        Task<bool> MarkAllReadAsync(int recipientId);
        Task<bool> AcknowledgeAsync(int alertId);
        Task<bool> DeleteAlertAsync(int alertId);
    }
}