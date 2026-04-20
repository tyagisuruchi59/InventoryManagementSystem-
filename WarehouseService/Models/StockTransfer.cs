// StockPro Inventory Management System
// Service: Warehouse Service | Model: StockTransfer
// Developer: Suru | April 2026
// Description: Records stock transfer between warehouses

namespace WarehouseService.Models
{
    public class StockTransfer
    {
        // Unique ID
        public int Id { get; set; }

        // Product being transferred
        public int ProductId { get; set; }

        // Source warehouse
        public int FromWarehouseId { get; set; }

        // Destination warehouse
        public int ToWarehouseId { get; set; }

        // Quantity transferred
        public int Quantity { get; set; }

        // Reason for transfer
        public string Reason { get; set; } = "";

        // Who performed the transfer
        public string TransferredBy { get; set; } = "";

        // When transfer happened
        public DateTime TransferredAt { get; set; } = DateTime.UtcNow;
    }
}