// StockPro Inventory Management System
// Service: Report Service | DTO: Snapshot
// Developer: Suru | April 2026
// Description: Data for creating inventory snapshot

namespace ReportService.DTOs
{
    public class SnapshotDto
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
    }
}