// StockPro Inventory Management System
// Service: Auth Service | DTO: Login
// Developer: Suru | April 2026
// Description: Data received from user during login

namespace AuthService.DTOs
{
    public class LoginDto
    {
        // Username entered at login
        public string Username { get; set; } = "";

        // Plain text password entered at login
        public string Password { get; set; } = "";
    }
}