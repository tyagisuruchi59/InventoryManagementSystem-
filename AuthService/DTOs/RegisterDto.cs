// StockPro Inventory Management System
// Service: Auth Service | DTO: Register
// Developer: Suru | April 2026
// Description: Data received from user during registration

namespace AuthService.DTOs
{
    public class RegisterDto
    {
        // Username chosen by user
        public string Username { get; set; } = "";

        // Plain text password (will be hashed before saving)
        public string Password { get; set; } = "";

        // Role assigned to this user
        public string Role { get; set; } = "Staff";
    }
}