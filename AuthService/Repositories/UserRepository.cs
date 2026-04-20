// StockPro Inventory Management System
// Service: Auth Service | Repository: Implementation
// Developer: Suru | April 2026
// Description: Actual database operations using Entity Framework Core

using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models;

namespace AuthService.Repositories
{
    public class UserRepository : IUserRepository
    {
        // Database context injected automatically
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Find user by username from database
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        // Get all users from database
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Add new user to database
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        // Update user in database
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        // Commit all changes to database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetByIdAsync(int id)
{
    return await _context.Users.FindAsync(id);
}
    }
}