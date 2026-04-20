// StockPro Inventory Management System
// Service: Purchase Service | DTO: ReceiveGoods
// Developer: Suru | April 2026
// Description: Data received when recording goods receipt (GRN)

namespace PurchaseService.DTOs
{
    public class ReceiveGoodsDto
    {
        public int PurchaseOrderId { get; set; }
        public List<ReceiveLineItemDto> ReceivedItems { get; set; } = new();
    }

    public class ReceiveLineItemDto
    {
        public int LineItemId { get; set; }
        public int ReceivedQty { get; set; }
    }
}