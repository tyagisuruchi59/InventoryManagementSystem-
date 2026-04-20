// StockPro Inventory Management System
// Service: Purchase Service | DTO: UpdatePO
// Developer: Suru | April 2026
// Description: Data received when updating a purchase order

namespace PurchaseService.DTOs
{
    public class UpdatePODto
    {
        public DateTime ExpectedDate { get; set; }
        public string Notes { get; set; } = "";
        public string ReferenceNumber { get; set; } = "";
    }
}