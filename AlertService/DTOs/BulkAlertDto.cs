// StockPro Inventory Management System
// Service: Alert Service | DTO: BulkAlert
// Developer: Suru | April 2026
// Description: Used when sending alert to multiple recipients

namespace AlertService.DTOs
{
    public class BulkAlertDto
    {
        // List of recipient user IDs
        public List<int> RecipientIds { get; set; } = new();
        public string Type { get; set; } = "";
        public string Severity { get; set; } = "INFO";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
    }
}