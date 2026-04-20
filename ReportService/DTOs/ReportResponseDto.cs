// StockPro Inventory Management System
// Service: Report Service | DTO: ReportResponse
// Developer: Suru | April 2026
// Description: Generic report response data

namespace ReportService.DTOs
{
    public class ReportResponseDto
    {
        public string ReportType { get; set; } = "";
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public object? Data { get; set; }
    }
}