// StockPro Inventory Management System
// Service: Purchase Service | DTO: CreatePO
// Developer: Suru | April 2026
// Description: Data received when creating a new purchase order

namespace PurchaseService.DTOs
{
    public class CreatePODto
    {
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public int CreatedById { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string ReferenceNumber { get; set; } = "";
        public string Notes { get; set; } = "";
        public List<CreateLineItemDto> LineItems { get; set; } = new();
    }

    public class CreateLineItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}