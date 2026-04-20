// StockPro Inventory Management System
// Service: Warehouse Service | DTO: TransferStock
// Developer: Suru | April 2026
// Description: Data received when transferring stock between warehouses

namespace WarehouseService.DTOs
{
    public class TransferStockDto
    {
        public int ProductId { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = "";
        public string TransferredBy { get; set; } = "";
    }
}