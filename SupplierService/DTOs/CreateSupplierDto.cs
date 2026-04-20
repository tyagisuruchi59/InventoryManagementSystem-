// StockPro Inventory Management System
// Service: Supplier Service | DTO: CreateSupplier
// Developer: Suru | April 2026
// Description: Data received from client when registering a new supplier.
// All fields required to create a complete supplier master record.

namespace SupplierService.DTOs
{
    public class CreateSupplierDto
    {
        // Supplier company name
        public string Name { get; set; } = "";

        // Primary contact person
        public string ContactPerson { get; set; } = "";

        // Contact email
        public string Email { get; set; } = "";

        // Contact phone
        public string Phone { get; set; } = "";

        // Full street address
        public string Address { get; set; } = "";

        // City for geo-filter
        public string City { get; set; } = "";

        // Country for geo-filter
        public string Country { get; set; } = "";

        // Tax ID — must be unique
        public string TaxId { get; set; } = "";

        // Payment terms e.g. NET-30
        public string PaymentTerms { get; set; } = "NET-30";

        // Lead time in days
        public int LeadTimeDays { get; set; }
    }
}