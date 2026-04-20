// StockPro Inventory Management System
// Service: Alert Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines database operations for Alert

using AlertService.Models;

namespace AlertService.Repositories
{
    public interface IAlertRepository
    {
        // Get all alerts for a specific recipient
        Task<List<Alert>> GetByRecipientAsync(int recipientId);

        // Get unread alerts for a recipient
        Task<List<Alert>> GetUnreadByRecipientAsync(int recipientId);

        // Get unacknowledged alerts
        Task<List<Alert>> GetUnacknowledgedAsync();

        // Get alerts by type
        Task<List<Alert>> GetByTypeAsync(string type);

        // Get alerts by severity
        Task<List<Alert>> GetBySeverityAsync(string severity);

        // Get alert by ID
        Task<Alert?> GetByIdAsync(int id);

        // Get all alerts
        Task<List<Alert>> GetAllAsync();

        // Count unread alerts for recipient
        Task<int> CountUnreadAsync(int recipientId);

        // Add new alert
        Task AddAsync(Alert alert);

        // Add multiple alerts at once
        Task AddRangeAsync(List<Alert> alerts);

        // Update alert
        Task UpdateAsync(Alert alert);

        // Delete alert
        Task DeleteAsync(int id);

        // Save changes
        Task SaveChangesAsync();
    }
}