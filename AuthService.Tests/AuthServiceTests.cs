// StockPro Inventory Management System
// Test: Auth Service | MSTest
// Developer: Suru | April 2026
// Description: Unit tests for AuthServiceImpl using Moq

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;

namespace AuthService.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _mockRepo;
        private AuthServiceImpl _authService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository>();
            _authService = new AuthServiceImpl(_mockRepo.Object);
        }

        // TEST 1 — Register with new username should succeed
        [TestMethod]
        public async Task Register_NewUser_ReturnsSuccess()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByUsernameAsync("admin"))
                     .ReturnsAsync((User?)null);

            var dto = new RegisterDto
            {
                Username = "admin",
                Password = "1234",
                Role = "Admin"
            };

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.AreEqual("User registered successfully", result);
        }

        // TEST 2 — Register with existing username should fail
        [TestMethod]
        public async Task Register_ExistingUser_ReturnsDuplicate()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByUsernameAsync("admin"))
                     .ReturnsAsync(new User { Username = "admin" });

            var dto = new RegisterDto
            {
                Username = "admin",
                Password = "1234",
                Role = "Admin"
            };

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.AreEqual("Username already exists", result);
        }

        // TEST 3 — Login with wrong password should return null
        [TestMethod]
        public async Task Login_WrongPassword_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByUsernameAsync("admin"))
                     .ReturnsAsync(new User
                     {
                         Username = "admin",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
                         IsActive = true,
                         Role = "Admin"
                     });

            var dto = new LoginDto
            {
                Username = "admin",
                Password = "wrongpassword"
            };

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.IsNull(result);
        }

        // TEST 4 — Login with correct password should return token
        [TestMethod]
        public async Task Login_CorrectPassword_ReturnsToken()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByUsernameAsync("admin"))
                     .ReturnsAsync(new User
                     {
                         Id = 1,
                         Username = "admin",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                         IsActive = true,
                         Role = "Admin"
                     });

            var dto = new LoginDto
            {
                Username = "admin",
                Password = "1234"
            };

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("admin", result.Username);
            Assert.IsFalse(string.IsNullOrEmpty(result.Token));
        }

        // TEST 5 — Login with inactive user should return null
        [TestMethod]
        public async Task Login_InactiveUser_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByUsernameAsync("admin"))
                     .ReturnsAsync(new User
                     {
                         Username = "admin",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                         IsActive = false,
                         Role = "Admin"
                     });

            var dto = new LoginDto
            {
                Username = "admin",
                Password = "1234"
            };

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.IsNull(result);
        }

        // TEST 6 — Get all users should return list
        [TestMethod]
        public async Task GetAllUsers_ReturnsUserList()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new List<User>
                     {
                         new User { Id = 1, Username = "admin", Role = "Admin" },
                         new User { Id = 2, Username = "staff", Role = "Staff" }
                     });

            // Act
            var result = await _authService.GetAllUsersAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
        }
        // TEST — Register with empty username
[TestMethod]
public async Task Register_EmptyUsername_ReturnsError()
{
    var dto = new RegisterDto { Username = "", Password = "1234" };

    var result = await _authService.RegisterAsync(dto);

    Assert.IsNotNull(result);
}

// TEST — Login with non-existing user
[TestMethod]
public async Task Login_UserNotFound_ReturnsNull()
{
    _mockRepo.Setup(r => r.GetByUsernameAsync("ghost"))
             .ReturnsAsync((User?)null);

    var result = await _authService.LoginAsync(new LoginDto
    {
        Username = "ghost",
        Password = "1234"
    });

    Assert.IsNull(result);
}
    }
}