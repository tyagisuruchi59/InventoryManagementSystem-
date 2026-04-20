// StockPro Inventory Management System
// Model: Warehouse
// Developer: Suru | April 2026
// Description: Represents a physical warehouse location

public class Warehouse
{
    // Unique ID for every warehouse
    public int Id { get; set; }

    // Name of the warehouse e.g. "Delhi Warehouse"
    public string Name { get; set; } = "";

    // Physical location/address
    public string Location { get; set; } = "";

    // Maximum capacity this warehouse can hold
    public int Capacity { get; set; }

    // Is this warehouse currently active?
    public bool IsActive { get; set; } = true;
}