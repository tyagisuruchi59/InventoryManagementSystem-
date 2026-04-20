// StockPro Inventory Management System
// Service: Auth Service | DTO: UserResponse
// Developer: Suru | April 2026
// Description: Data sent back to user after login - never includes password

namespace AuthService.DTOs
{
    public class UserResponseDto
    {
        // User ID
        public int Id { get; set; }

        // Username
        public string Username { get; set; } = "";

        // User role
        public string Role { get; set; } = "";

        // JWT token for accessing other services
        public string Token { get; set; } = "";
    }
}