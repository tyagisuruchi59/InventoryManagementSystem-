// StockPro Inventory Management System
// Service: Alert Service | DTO: CreateAlert
// Developer: Suru | April 2026
// Description: Data received when creating a new alert

namespace AlertService.DTOs
{
    public class CreateAlertDto
    {
        public int RecipientId { get; set; }
        public string Type { get; set; } = "";
        public string Severity { get; set; } = "INFO";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public int? RelatedProductId { get; set; }
        public int? RelatedWarehouseId { get; set; }
        public string Channel { get; set; } = "IN_APP";
    }
}