// StockPro Inventory Management System
// Service: Auth Service | DTO: ChangePassword
// Developer: Suru | April 2026
// Description: Used when user wants to change their password

namespace AuthService.DTOs
{
    public class ChangePasswordDto
    {
        // Current password for verification
        public string CurrentPassword { get; set; } = "";

        // New password to set
        public string NewPassword { get; set; } = "";
    }
}