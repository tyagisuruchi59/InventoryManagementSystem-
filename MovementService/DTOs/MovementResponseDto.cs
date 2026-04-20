// StockPro Inventory Management System
// Service: Movement Service | DTO: MovementResponse
// Developer: Suru | April 2026
// Description: Data returned to client for movement queries.
// Full movement details including BalanceAfter for stock reconstruction.

namespace MovementService.DTOs
{
    public class MovementResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public string MovementType { get; set; } = "";
        public int Quantity { get; set; }
        public string ReferenceId { get; set; } = "";
        public string ReferenceType { get; set; } = "";
        public decimal UnitCost { get; set; }
        public string PerformedBy { get; set; } = "";
        public string Notes { get; set; } = "";
        public DateTime MovementDate { get; set; }
        public int BalanceAfter { get; set; }
    }
}