// StockPro Inventory Management System
// Service: Auth Service | Controller: Auth
// Developer: Suru | April 2026
// Description: Full authentication - register, login, user management, role management

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthService.DTOs;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/register - Register new user (Admin only)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == "Username already exists")
                return BadRequest(result);
            return Ok(result);
        }

        // POST /api/auth/login - Login and get JWT token (public)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized("Invalid username or password");
            return Ok(result);
        }

        // GET /api/auth/users - Get all users (Admin only)
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET /api/auth/users/{id} - Get single user by ID (Admin only)
        [HttpGet("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        // PUT /api/auth/users/{id}/role - Change user role (Admin only)
        [HttpPut("users/{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto dto)
        {
            var result = await _authService.UpdateRoleAsync(id, dto.Role);
            if (!result) return NotFound("User not found");
            return Ok("Role updated successfully");
        }

        // PUT /api/auth/users/{id}/deactivate - Deactivate user (Admin only)
        [HttpPut("users/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var result = await _authService.DeactivateUserAsync(id);
            if (!result) return NotFound("User not found");
            return Ok("User deactivated successfully");
        }

        // GET /api/auth/me - Get current logged in user profile
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            // Get username and role from JWT token claims
            var username = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c =>
                c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new { Username = username, Role = role });
        }

        // PUT /api/auth/change-password - Change own password
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var username = User.Identity?.Name;
            var result = await _authService.ChangePasswordAsync(username!, dto);
            if (!result) return BadRequest("Current password is incorrect");
            return Ok("Password changed successfully");
        }
    }
}