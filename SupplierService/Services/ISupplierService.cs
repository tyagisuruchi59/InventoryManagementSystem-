// StockPro Inventory Management System
// Service: Supplier Service | Service: Interface
// Developer: Suru | April 2026
// Description: Declares all business logic operations for supplier management.
// Covers CRUD, search, deactivation, rating update, and geo-filter operations
// as specified in the documentation class diagram.

using SupplierService.DTOs;

namespace SupplierService.Services
{
    public interface ISupplierService
    {
        // Create a new supplier
        Task<string> CreateSupplierAsync(CreateSupplierDto dto);

        // Get supplier by ID
        Task<SupplierResponseDto?> GetByIdAsync(int id);

        // Get all suppliers
        Task<List<SupplierResponseDto>> GetAllSuppliersAsync();

        // Search suppliers by name
        Task<List<SupplierResponseDto>> SearchSuppliersAsync(string name);

        // Update supplier details
        Task<bool> UpdateSupplierAsync(int id, UpdateSupplierDto dto);

        // Soft deactivate supplier — preserves history
        Task<bool> DeactivateSupplierAsync(int id);

        // Hard delete supplier
        Task<bool> DeleteSupplierAsync(int id);

        // Get suppliers by city — geo-filter
        Task<List<SupplierResponseDto>> GetByCityAsync(string city);

        // Get suppliers by country — geo-filter
        Task<List<SupplierResponseDto>> GetByCountryAsync(string country);

        // Update supplier performance rating after goods receipt
        Task<bool> UpdateRatingAsync(int id, UpdateRatingDto dto);

        // Get count of active suppliers
        Task<int> GetActiveCountAsync();
    }
}