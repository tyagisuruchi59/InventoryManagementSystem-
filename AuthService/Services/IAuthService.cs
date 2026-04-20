// StockPro Inventory Management System
// Service: Auth Service | Service: Interface
// Developer: Suru | April 2026
// Description: Defines all business logic operations for authentication

using AuthService.DTOs;

namespace AuthService.Services
{
    public interface IAuthService
    {
        // Register a new user
        Task<string> RegisterAsync(RegisterDto dto);

        // Login and return JWT token
        Task<UserResponseDto?> LoginAsync(LoginDto dto);

        // Get all users (Admin only)
        Task<List<UserResponseDto>> GetAllUsersAsync();

        // Get single user by ID
        Task<UserResponseDto?> GetUserByIdAsync(int id);

        // Update user role
        Task<bool> UpdateRoleAsync(int id, string newRole);

        // Deactivate user account
        Task<bool> DeactivateUserAsync(int id);

        // Change password
        Task<bool> ChangePasswordAsync(string username, ChangePasswordDto dto);
    }
}