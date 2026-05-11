namespace AlertService.Events
{
    public class POApprovedEvent
    {
        public int POId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public decimal TotalAmount { get; set; }
        public string ApprovedBy { get; set; } = string.Empty;
        public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
    }
}