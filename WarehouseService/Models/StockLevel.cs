// StockPro Inventory Management System
// Service: Warehouse Service | Model: StockLevel
// Developer: Suru | April 2026
// Description: Tracks stock quantity per product per warehouse

namespace WarehouseService.Models
{
    public class StockLevel
    {
        // Unique ID
        public int Id { get; set; }

        // Which warehouse this stock belongs to
        public int WarehouseId { get; set; }

        // Which product this stock is for
        public int ProductId { get; set; }

        // Total quantity physically present
        public int Quantity { get; set; }

        // Reserved quantity for open orders
        public int ReservedQuantity { get; set; }

        // Available = Total - Reserved
        public int AvailableQuantity => Quantity - ReservedQuantity;

        // Reorder level for this product in this warehouse
        public int ReorderLevel { get; set; }

        // Maximum stock level for this product
        public int MaxStockLevel { get; set; }

        // Last updated timestamp
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}