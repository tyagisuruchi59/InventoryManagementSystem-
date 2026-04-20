// StockPro Inventory Management System
// Service: Supplier Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Implements all database operations using EF Core and PostgreSQL.
// Handles all CRUD, search, geo-filter, and count operations for Supplier entity.

using Microsoft.EntityFrameworkCore;
using SupplierService.Data;
using SupplierService.Models;

namespace SupplierService.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        // EF Core DbContext injected via constructor DI
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        // Find single supplier by ID
        public async Task<Supplier?> FindBySupplierIdAsync(int id)
            => await _context.Suppliers.FindAsync(id);

        // Find all suppliers in a city — geo-filter feature
        public async Task<List<Supplier>> FindByCityAsync(string city)
            => await _context.Suppliers
                .Where(s => s.City.ToLower() == city.ToLower())
                .ToListAsync();

        // Find all suppliers in a country — geo-filter feature
        public async Task<List<Supplier>> FindByCountryAsync(string country)
            => await _context.Suppliers
                .Where(s => s.Country.ToLower() == country.ToLower())
                .ToListAsync();

        // Search suppliers by name — partial match, case insensitive
        public async Task<List<Supplier>> SearchByNameAsync(string name)
            => await _context.Suppliers
                .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

        // Find suppliers by active status
        public async Task<List<Supplier>> FindByIsActiveAsync(bool isActive)
            => await _context.Suppliers
                .Where(s => s.IsActive == isActive)
                .ToListAsync();

        // Find supplier by Tax ID — unique identifier
        public async Task<Supplier?> FindByTaxIdAsync(string taxId)
            => await _context.Suppliers
                .FirstOrDefaultAsync(s => s.TaxId == taxId);

        // Count suppliers by active status
        public async Task<int> CountByIsActiveAsync(bool isActive)
            => await _context.Suppliers
                .CountAsync(s => s.IsActive == isActive);

        // Get all suppliers
        public async Task<List<Supplier>> GetAllAsync()
            => await _context.Suppliers.ToListAsync();

        // Add new supplier to database
        public async Task AddAsync(Supplier supplier)
            => await _context.Suppliers.AddAsync(supplier);

        // Update existing supplier
        public async Task UpdateAsync(Supplier supplier)
            => _context.Suppliers.Update(supplier);

        // Hard delete supplier by ID
        public async Task<bool> DeleteBySupplierIdAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;
            _context.Suppliers.Remove(supplier);
            return true;
        }

        // Commit all changes to database
        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}