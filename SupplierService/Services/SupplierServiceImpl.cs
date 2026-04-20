// StockPro Inventory Management System
// Service: Supplier Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Implements all business logic for supplier management.
// Handles supplier creation with duplicate TaxId check, soft deactivation
// to preserve PO history, rating updates after goods receipt, and
// geo-filter queries by city and country.

using SupplierService.DTOs;
using SupplierService.Models;
using SupplierService.Repositories;

namespace SupplierService.Services
{
    public class SupplierServiceImpl : ISupplierService
    {
        // Repository injected via constructor dependency injection
        private readonly ISupplierRepository _repo;

        public SupplierServiceImpl(ISupplierRepository repo)
        {
            _repo = repo;
        }

        // CREATE SUPPLIER
        // Validates TaxId uniqueness before saving
        public async Task<string> CreateSupplierAsync(CreateSupplierDto dto)
        {
            // Check if TaxId already exists — must be unique per supplier
            var existing = await _repo.FindByTaxIdAsync(dto.TaxId);
            if (existing != null)
                return "Supplier with this Tax ID already exists";

            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                TaxId = dto.TaxId,
                PaymentTerms = dto.PaymentTerms,
                LeadTimeDays = dto.LeadTimeDays,
                Rating = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(supplier);
            await _repo.SaveChangesAsync();
            return "Supplier created successfully";
        }

        // GET BY ID
        public async Task<SupplierResponseDto?> GetByIdAsync(int id)
        {
            var supplier = await _repo.FindBySupplierIdAsync(id);
            return supplier == null ? null : MapToDto(supplier);
        }

        // GET ALL SUPPLIERS
        public async Task<List<SupplierResponseDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _repo.GetAllAsync();
            return suppliers.Select(MapToDto).ToList();
        }

        // SEARCH BY NAME — partial match
        public async Task<List<SupplierResponseDto>> SearchSuppliersAsync(string name)
        {
            var suppliers = await _repo.SearchByNameAsync(name);
            return suppliers.Select(MapToDto).ToList();
        }

        // UPDATE SUPPLIER DETAILS
        public async Task<bool> UpdateSupplierAsync(int id, UpdateSupplierDto dto)
        {
            var supplier = await _repo.FindBySupplierIdAsync(id);
            if (supplier == null) return false;

            supplier.Name = dto.Name;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Email = dto.Email;
            supplier.Phone = dto.Phone;
            supplier.Address = dto.Address;
            supplier.City = dto.City;
            supplier.Country = dto.Country;
            supplier.PaymentTerms = dto.PaymentTerms;
            supplier.LeadTimeDays = dto.LeadTimeDays;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(supplier);
            await _repo.SaveChangesAsync();
            return true;
        }

        // DEACTIVATE SUPPLIER
        // Soft delete — sets IsActive = false, preserves all PO history
        // Inactive suppliers cannot receive new purchase orders
        public async Task<bool> DeactivateSupplierAsync(int id)
        {
            var supplier = await _repo.FindBySupplierIdAsync(id);
            if (supplier == null) return false;

            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(supplier);
            await _repo.SaveChangesAsync();
            return true;
        }

        // DELETE SUPPLIER — hard delete from database
        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var result = await _repo.DeleteBySupplierIdAsync(id);
            if (!result) return false;
            await _repo.SaveChangesAsync();
            return true;
        }

        // GET BY CITY — geo-filter feature
        public async Task<List<SupplierResponseDto>> GetByCityAsync(string city)
        {
            var suppliers = await _repo.FindByCityAsync(city);
            return suppliers.Select(MapToDto).ToList();
        }

        // GET BY COUNTRY — geo-filter feature
        public async Task<List<SupplierResponseDto>> GetByCountryAsync(string country)
        {
            var suppliers = await _repo.FindByCountryAsync(country);
            return suppliers.Select(MapToDto).ToList();
        }

        // UPDATE RATING
        // Called after goods receipt in Purchase Service
        // Rating must be between 0.0 and 5.0
        public async Task<bool> UpdateRatingAsync(int id, UpdateRatingDto dto)
        {
            var supplier = await _repo.FindBySupplierIdAsync(id);
            if (supplier == null) return false;

            // Validate rating range
            if (dto.Rating < 0 || dto.Rating > 5)
                return false;

            supplier.Rating = dto.Rating;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(supplier);
            await _repo.SaveChangesAsync();
            return true;
        }

        // GET COUNT OF ACTIVE SUPPLIERS
        public async Task<int> GetActiveCountAsync()
            => await _repo.CountByIsActiveAsync(true);

        // MAP Supplier model to SupplierResponseDto
        private SupplierResponseDto MapToDto(Supplier supplier)
        {
            return new SupplierResponseDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                City = supplier.City,
                Country = supplier.Country,
                TaxId = supplier.TaxId,
                PaymentTerms = supplier.PaymentTerms,
                LeadTimeDays = supplier.LeadTimeDays,
                Rating = supplier.Rating,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }
    }
}