// StockPro Inventory Management System
// Service: Supplier Service | DTO: UpdateSupplier
// Developer: Suru | April 2026
// Description: Data received when updating an existing supplier record.
// TaxId and Rating are not updatable here — use separate endpoints.

namespace SupplierService.DTOs
{
    public class UpdateSupplierDto
    {
        // Updated company name
        public string Name { get; set; } = "";

        // Updated contact person
        public string ContactPerson { get; set; } = "";

        // Updated email
        public string Email { get; set; } = "";

        // Updated phone
        public string Phone { get; set; } = "";

        // Updated address
        public string Address { get; set; } = "";

        // Updated city
        public string City { get; set; } = "";

        // Updated country
        public string Country { get; set; } = "";

        // Updated payment terms
        public string PaymentTerms { get; set; } = "NET-30";

        // Updated lead time
        public int LeadTimeDays { get; set; }
    }
}