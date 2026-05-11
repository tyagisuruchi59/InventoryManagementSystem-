namespace PurchaseService.Events
{
    public class POSubmittedEvent
    {
        public int POId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public decimal TotalAmount { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
