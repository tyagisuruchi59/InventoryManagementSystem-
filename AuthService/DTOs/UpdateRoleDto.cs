// StockPro Inventory Management System
// Service: Auth Service | DTO: UpdateRole
// Developer: Suru | April 2026
// Description: Used when admin changes a user's role

namespace AuthService.DTOs
{
    public class UpdateRoleDto
    {
        // New role to assign: Admin, Manager, Staff, Viewer
        public string Role { get; set; } = "";
    }
}