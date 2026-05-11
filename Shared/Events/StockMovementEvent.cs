namespace Shared.Events
{
    public class StockMovementEvent
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    }
}