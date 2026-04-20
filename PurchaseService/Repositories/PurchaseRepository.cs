// StockPro Inventory Management System
// Service: Purchase Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Database operations using Entity Framework Core

using Microsoft.EntityFrameworkCore;
using PurchaseService.Data;
using PurchaseService.Models;

namespace PurchaseService.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext _context;

        public PurchaseRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all POs with their line items
        public async Task<List<PurchaseOrder>> GetAllAsync()
            => await _context.PurchaseOrders
                .Include(p => p.LineItems)
                .ToListAsync();

        // Get single PO with line items
        public async Task<PurchaseOrder?> GetByIdAsync(int id)
            => await _context.PurchaseOrders
                .Include(p => p.LineItems)
                .FirstOrDefaultAsync(p => p.Id == id);

        // Get POs by supplier
        public async Task<List<PurchaseOrder>> GetBySupplierAsync(int supplierId)
            => await _context.PurchaseOrders
                .Include(p => p.LineItems)
                .Where(p => p.SupplierId == supplierId)
                .ToListAsync();

        // Get POs by warehouse
        public async Task<List<PurchaseOrder>> GetByWarehouseAsync(int warehouseId)
            => await _context.PurchaseOrders
                .Include(p => p.LineItems)
                .Where(p => p.WarehouseId == warehouseId)
                .ToListAsync();

        // Get POs by status e.g. Draft, Approved
        public async Task<List<PurchaseOrder>> GetByStatusAsync(string status)
            => await _context.PurchaseOrders
                .Include(p => p.LineItems)
                .Where(p => p.Status == status)
                .ToListAsync();

        // Get POs created by specific user
        public async Task<List<PurchaseOrder>> GetByCreatedByAsync(int userId)
            => await _context.PurchaseOrders
                .Include(p => p.LineItems)
                .Where(p => p.CreatedById == userId)
                .ToListAsync();

        // Get POs within date range
        public async Task<List<PurchaseOrder>> GetByDateRangeAsync(DateTime from, DateTime to)
            => await _context.PurchaseOrders
                .Include(p => p.LineItems)
                .Where(p => p.OrderDate >= from && p.OrderDate <= to)
                .ToListAsync();

        public async Task AddAsync(PurchaseOrder po)
            => await _context.PurchaseOrders.AddAsync(po);

        public async Task UpdateAsync(PurchaseOrder po)
            => _context.PurchaseOrders.Update(po);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}