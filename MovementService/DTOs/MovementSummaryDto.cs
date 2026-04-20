// StockPro Inventory Management System
// Service: Movement Service | DTO: MovementSummary
// Developer: Suru | April 2026
// Description: Summary totals for stock in and stock out queries.
// Used for analytics and dashboard widgets.

namespace MovementService.DTOs
{
    public class MovementSummaryDto
    {
        // Product ID this summary is for
        public int ProductId { get; set; }

        // Warehouse ID this summary is for
        public int WarehouseId { get; set; }

        // Total units received (STOCK_IN + TRANSFER_IN + RETURN)
        public int TotalStockIn { get; set; }

        // Total units issued (STOCK_OUT + TRANSFER_OUT + WRITE_OFF)
        public int TotalStockOut { get; set; }

        // Net movement = StockIn - StockOut
        public int NetMovement { get; set; }
    }
}