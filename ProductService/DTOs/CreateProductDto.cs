// StockPro Inventory Management System
// Service: Product Service | DTO: CreateProduct
// Developer: Suru | April 2026
// Description: Data received when creating a new product

namespace ProductService.DTOs
{
    public class CreateProductDto
    {
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
    }
}