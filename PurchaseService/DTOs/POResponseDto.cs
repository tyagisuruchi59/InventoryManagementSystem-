// StockPro Inventory Management System
// Service: Purchase Service | DTO: POResponse
// Developer: Suru | April 2026
// Description: Data sent back to client for purchase order queries

namespace PurchaseService.DTOs
{
    public class POResponseDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public int CreatedById { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string ReferenceNumber { get; set; } = "";
        public string Notes { get; set; } = "";
        public List<LineItemResponseDto> LineItems { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class LineItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public int ReceivedQty { get; set; }
    }
}