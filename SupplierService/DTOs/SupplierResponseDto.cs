// StockPro Inventory Management System
// Service: Supplier Service | DTO: SupplierResponse
// Developer: Suru | April 2026
// Description: Data returned to client after supplier operations.
// Full supplier details including rating and active status.

namespace SupplierService.DTOs
{
    public class SupplierResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ContactPerson { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string TaxId { get; set; } = "";
        public string PaymentTerms { get; set; } = "";
        public int LeadTimeDays { get; set; }
        public decimal Rating { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}