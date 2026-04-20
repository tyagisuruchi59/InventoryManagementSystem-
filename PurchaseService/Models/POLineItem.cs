// StockPro Inventory Management System
// Service: Purchase Service | Model: POLineItem
// Developer: Suru | April 2026
// Description: Represents a single product line in a purchase order

namespace PurchaseService.Models
{
    public class POLineItem
    {
        // Unique ID
        public int Id { get; set; }

        // Which PO this line belongs to
        public int PurchaseOrderId { get; set; }

        // Which product is being ordered
        public int ProductId { get; set; }

        // Quantity ordered
        public int Quantity { get; set; }

        // Unit cost at time of ordering
        public decimal UnitCost { get; set; }

        // Total cost = Quantity x UnitCost
        public decimal TotalCost => Quantity * UnitCost;

        // Quantity already received (for partial receipts)
        public int ReceivedQty { get; set; } = 0;

        // Navigation property
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}