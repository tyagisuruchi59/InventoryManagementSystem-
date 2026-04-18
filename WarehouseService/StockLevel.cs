// StockPro Inventory Management System
// Model: StockLevel
// Developer: Suru | April 2026
// Description: Tracks how much of each product is in each warehouse

public class StockLevel
{
    // Unique ID for this stock record
    public int Id { get; set; }

    // Which warehouse this stock belongs to
    public int WarehouseId { get; set; }

    // Which product this stock is for
    public int ProductId { get; set; }

    // Total quantity physically present
    public int Quantity { get; set; }

    // Quantity reserved for open orders (cannot be used)
    public int ReservedQuantity { get; set; }

    // Available = Total - Reserved (calculated property)
    public int AvailableQuantity => Quantity - ReservedQuantity;
}