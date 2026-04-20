// StockPro Inventory Management System
// Service: Warehouse Service | DTO: StockLevel
// Developer: Suru | April 2026
// Description: Data for adding or updating stock levels

namespace WarehouseService.DTOs
{
    public class StockLevelDto
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public int MaxStockLevel { get; set; }
    }
}