// StockPro Inventory Management System
// Service: Product Service | DTO: ProductResponse
// Developer: Suru | April 2026
// Description: Data sent back to client after product operations

namespace ProductService.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public string Brand { get; set; } = "";
        public string UnitOfMeasure { get; set; } = "";
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string Barcode { get; set; } = "";
        public int ReorderLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public int LeadTimeDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}