// StockPro Inventory Management System
// Service: Supplier Service | DTO: UpdateRating
// Developer: Suru | April 2026
// Description: Data received when updating supplier performance rating.
// Rating is updated separately after each goods receipt from Purchase Service.

namespace SupplierService.DTOs
{
    public class UpdateRatingDto
    {
        // New rating value between 0.0 and 5.0
        public decimal Rating { get; set; }
    }
}