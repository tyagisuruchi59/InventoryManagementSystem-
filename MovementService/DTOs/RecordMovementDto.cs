// StockPro Inventory Management System
// Service: Movement Service | DTO: RecordMovement
// Developer: Suru | April 2026
// Description: Data received when recording a new stock movement.
// Used for all movement types: STOCK_IN, STOCK_OUT, TRANSFER_IN,
// TRANSFER_OUT, ADJUSTMENT, WRITE_OFF, RETURN.

namespace MovementService.DTOs
{
    public class RecordMovementDto
    {
        // Product being moved
        public int ProductId { get; set; }

        // Warehouse where movement occurs
        public int WarehouseId { get; set; }

        // Movement type — must be one of the defined types
        public string MovementType { get; set; } = "";

        // Quantity moved — must be positive
        public int Quantity { get; set; }

        // Reference ID — PO ID, issue order ID etc
        public string ReferenceId { get; set; } = "";

        // Reference type — PO, ISSUE, TRANSFER, AUDIT
        public string ReferenceType { get; set; } = "";

        // Unit cost at time of movement
        public decimal UnitCost { get; set; }

        // Who is performing this movement
        public string PerformedBy { get; set; } = "";

        // Notes or reason for movement
        public string Notes { get; set; } = "";

        // Stock balance after this movement
        public int BalanceAfter { get; set; }
    }
}