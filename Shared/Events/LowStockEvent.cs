namespace Shared.Events
{
    public class LowStockEvent
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    }
}