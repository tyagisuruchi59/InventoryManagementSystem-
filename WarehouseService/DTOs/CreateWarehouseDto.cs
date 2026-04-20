// StockPro Inventory Management System
// Service: Warehouse Service | DTO: CreateWarehouse
// Developer: Suru | April 2026
// Description: Data received when creating a new warehouse

namespace WarehouseService.DTOs
{
    public class CreateWarehouseDto
    {
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public int Capacity { get; set; }
        public int? ManagerId { get; set; }
    }
}