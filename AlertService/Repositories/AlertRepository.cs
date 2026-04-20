// StockPro Inventory Management System
// Service: Alert Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Database operations using Entity Framework Core

using Microsoft.EntityFrameworkCore;
using AlertService.Data;
using AlertService.Models;

namespace AlertService.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly AppDbContext _context;

        public AlertRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alert>> GetByRecipientAsync(int recipientId)
            => await _context.Alerts
                .Where(a => a.RecipientId == recipientId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<List<Alert>> GetUnreadByRecipientAsync(int recipientId)
            => await _context.Alerts
                .Where(a => a.RecipientId == recipientId && !a.IsRead)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<List<Alert>> GetUnacknowledgedAsync()
            => await _context.Alerts
                .Where(a => !a.IsAcknowledged)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<List<Alert>> GetByTypeAsync(string type)
            => await _context.Alerts
                .Where(a => a.Type == type)
                .ToListAsync();

        public async Task<List<Alert>> GetBySeverityAsync(string severity)
            => await _context.Alerts
                .Where(a => a.Severity == severity)
                .ToListAsync();

        public async Task<Alert?> GetByIdAsync(int id)
            => await _context.Alerts.FindAsync(id);

        public async Task<List<Alert>> GetAllAsync()
            => await _context.Alerts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public async Task<int> CountUnreadAsync(int recipientId)
            => await _context.Alerts
                .CountAsync(a => a.RecipientId == recipientId && !a.IsRead);

        public async Task AddAsync(Alert alert)
            => await _context.Alerts.AddAsync(alert);

        public async Task AddRangeAsync(List<Alert> alerts)
            => await _context.Alerts.AddRangeAsync(alerts);

        public async Task UpdateAsync(Alert alert)
            => _context.Alerts.Update(alert);

        public async Task DeleteAsync(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert != null)
                _context.Alerts.Remove(alert);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}