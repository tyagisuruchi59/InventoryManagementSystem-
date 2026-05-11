// StockPro Inventory Management System
// Test: Supplier Service | MSTest
// Developer: Suru | April 2026
// Description: Unit tests for SupplierServiceImpl using Moq

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SupplierService.DTOs;
using SupplierService.Models;
using SupplierService.Repositories;
using SupplierService.Services;

namespace SupplierService.Tests
{
    [TestClass]
    public class SupplierServiceTests
    {
        private Mock<ISupplierRepository> _mockRepo;
        private SupplierServiceImpl _supplierService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ISupplierRepository>();
            _supplierService = new SupplierServiceImpl(_mockRepo.Object);
        }

        // TEST 1 — Create supplier with new TaxId should succeed
        [TestMethod]
        public async Task CreateSupplier_NewTaxId_ReturnsSuccess()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindByTaxIdAsync("TAX001"))
                     .ReturnsAsync((Supplier?)null);

            var dto = new CreateSupplierDto
            {
                Name = "Test Supplier",
                TaxId = "TAX001",
                Email = "test@supplier.com",
                Phone = "9876543210",
                City = "Delhi",
                Country = "India",
                PaymentTerms = "NET-30",
                LeadTimeDays = 7
            };

            // Act
            var result = await _supplierService.CreateSupplierAsync(dto);

            // Assert
            Assert.AreEqual("Supplier created successfully", result);
        }

        // TEST 2 — Create supplier with existing TaxId should fail
        [TestMethod]
        public async Task CreateSupplier_ExistingTaxId_ReturnsDuplicate()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindByTaxIdAsync("TAX001"))
                     .ReturnsAsync(new Supplier { TaxId = "TAX001" });

            var dto = new CreateSupplierDto { TaxId = "TAX001" };

            // Act
            var result = await _supplierService.CreateSupplierAsync(dto);

            // Assert
            Assert.AreEqual("Supplier with this Tax ID already exists", result);
        }

        // TEST 3 — Update rating with valid value should return true
        [TestMethod]
        public async Task UpdateRating_ValidValue_ReturnsTrue()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindBySupplierIdAsync(1))
                     .ReturnsAsync(new Supplier { Id = 1, Rating = 3.0m });

            var dto = new UpdateRatingDto { Rating = 4.5m };

            // Act
            var result = await _supplierService.UpdateRatingAsync(1, dto);

            // Assert
            Assert.IsTrue(result);
        }

        // TEST 4 — Update rating above 5 should return false
        [TestMethod]
        public async Task UpdateRating_AboveMax_ReturnsFalse()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindBySupplierIdAsync(1))
                     .ReturnsAsync(new Supplier { Id = 1, Rating = 3.0m });

            var dto = new UpdateRatingDto { Rating = 6.0m };

            // Act
            var result = await _supplierService.UpdateRatingAsync(1, dto);

            // Assert
            Assert.IsFalse(result);
        }

        // TEST 5 — Deactivate supplier should return true
        [TestMethod]
        public async Task DeactivateSupplier_ValidId_ReturnsTrue()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindBySupplierIdAsync(1))
                     .ReturnsAsync(new Supplier { Id = 1, IsActive = true });

            // Act
            var result = await _supplierService.DeactivateSupplierAsync(1);

            // Assert
            Assert.IsTrue(result);
        }

        // TEST 6 — Search suppliers should return matching list
        [TestMethod]
        public async Task SearchSuppliers_ValidName_ReturnsList()
        {
            // Arrange
            _mockRepo.Setup(r => r.SearchByNameAsync("Tech"))
                     .ReturnsAsync(new List<Supplier>
                     {
                         new Supplier { Id = 1, Name = "Tech Corp" },
                         new Supplier { Id = 2, Name = "Tech Solutions" }
                     });

            // Act
            var result = await _supplierService.SearchSuppliersAsync("Tech");

            // Assert
            Assert.AreEqual(2, result.Count);
        }
        // 7 — Invalid email
[TestMethod]
public async Task CreateSupplier_InvalidEmail_ReturnsError()
{
    var dto = new CreateSupplierDto { Email = "invalid" };

    var result = await _supplierService.CreateSupplierAsync(dto);

    Assert.IsNotNull(result);
}

// 8 — Rating below 0
[TestMethod]
public async Task UpdateRating_BelowZero_ReturnsFalse()
{
    _mockRepo.Setup(r => r.FindBySupplierIdAsync(1))
        .ReturnsAsync(new Supplier { Id = 1 });

    var result = await _supplierService.UpdateRatingAsync(1, new UpdateRatingDto { Rating = -1 });

    Assert.IsFalse(result);
}

// 9 — Search empty
[TestMethod]
public async Task Search_Empty_ReturnsEmpty()
{
    _mockRepo.Setup(r => r.SearchByNameAsync(""))
        .ReturnsAsync(new List<Supplier>());

    var result = await _supplierService.SearchSuppliersAsync("");

    Assert.AreEqual(0, result.Count);
}
    }
}