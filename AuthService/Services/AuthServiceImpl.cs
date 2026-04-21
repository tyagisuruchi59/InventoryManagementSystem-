// StockPro Inventory Management System
// Service: Auth Service | Service: Implementation
// Developer: Suru | April 2026
// Description: Full business logic for authentication and user management

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories;

namespace AuthService.Services
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtKey = "MyVeryStrongSecretKeyForJwtAuth1234567890Secure";

        public AuthServiceImpl(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // REGISTER: Hash password and save to database
        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existing != null)
                return "Username already exists";

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role.ToUpper(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return "User registered successfully";
        }

        // LOGIN: Verify password and return JWT token
        public async Task<UserResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null || !user.IsActive) return null;
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };
        }

        // GET ALL USERS
        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                Token = ""
            }).ToList();
        }

        // GET USER BY ID
        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Token = ""
            };
        }

        // UPDATE ROLE
        public async Task<bool> UpdateRoleAsync(int id, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;
            user.Role = newRole;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        // DEACTIVATE USER
        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;
            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        // CHANGE PASSWORD
        public async Task<bool> ChangePasswordAsync(string username, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return false;
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash)) return false;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        // GENERATE JWT TOKEN
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}