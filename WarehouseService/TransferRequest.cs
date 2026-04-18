// StockPro Inventory Management System
// Model: TransferRequest
// Developer: Suru | April 2026
// Description: Used when moving stock from one warehouse to another

public class TransferRequest
{
    // Product to transfer
    public int ProductId { get; set; }

    // Where stock is coming FROM
    public int FromWarehouseId { get; set; }

    // Where stock is going TO
    public int ToWarehouseId { get; set; }

    // How many units to move
    public int Quantity { get; set; }
}