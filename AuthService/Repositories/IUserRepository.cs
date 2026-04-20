// StockPro Inventory Management System
// Service: Auth Service | Repository: Interface
// Developer: Suru | April 2026
// Description: Defines what database operations are available for User

using AuthService.Models;

namespace AuthService.Repositories
{
    public interface IUserRepository
    {
        // Find user by username
        Task<User?> GetByUsernameAsync(string username);

        // Get all users
        Task<List<User>> GetAllAsync();

        // Save new user to database
        Task AddAsync(User user);

        // Update existing user
        Task UpdateAsync(User user);
        Task<User?> GetByIdAsync(int id);

        // Save changes to database
        Task SaveChangesAsync();
    }
}