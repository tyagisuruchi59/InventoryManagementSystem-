// StockPro Inventory Management System
// Service: Supplier Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines all database operations for Supplier entity.
// Follows Repository pattern — separates data access from business logic.
// Methods match documentation: FindBySupplierId, FindByCity, FindByCountry,
// SearchByName, FindByIsActive, FindByTaxId, CountByIsActive, DeleteBySupplierId.

using SupplierService.Models;

namespace SupplierService.Repositories
{
    public interface ISupplierRepository
    {
        // Find supplier by their unique ID
        Task<Supplier?> FindBySupplierIdAsync(int id);

        // Find all suppliers in a specific city
        Task<List<Supplier>> FindByCityAsync(string city);

        // Find all suppliers in a specific country
        Task<List<Supplier>> FindByCountryAsync(string country);

        // Search suppliers by name — partial match
        Task<List<Supplier>> SearchByNameAsync(string name);

        // Find all active or inactive suppliers
        Task<List<Supplier>> FindByIsActiveAsync(bool isActive);

        // Find supplier by Tax ID — must be unique
        Task<Supplier?> FindByTaxIdAsync(string taxId);

        // Count how many active/inactive suppliers exist
        Task<int> CountByIsActiveAsync(bool isActive);

        // Get all suppliers
        Task<List<Supplier>> GetAllAsync();

        // Add new supplier
        Task AddAsync(Supplier supplier);

        // Update existing supplier
        Task UpdateAsync(Supplier supplier);

        // Hard delete supplier by ID
        Task<bool> DeleteBySupplierIdAsync(int id);

        // Save changes to database
        Task SaveChangesAsync();
    }
}