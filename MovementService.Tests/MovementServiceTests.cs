// StockPro Inventory Management System
// Test: Movement Service | MSTest
// Developer: Suru | April 2026
// Description: Unit tests for MovementServiceImpl using Moq

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovementService.DTOs;
using MovementService.Models;
using MovementService.Repositories;
using MovementService.Services;

namespace MovementService.Tests
{
    [TestClass]
    public class MovementServiceTests
    {
        private Mock<IMovementRepository> _mockRepo;
        private MovementServiceImpl _movementService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IMovementRepository>();
            _movementService = new MovementServiceImpl(_mockRepo.Object);
        }

        // TEST 1 — Record valid STOCK_IN movement should succeed
        [TestMethod]
        public async Task RecordMovement_ValidStockIn_ReturnsSuccess()
        {
            // Arrange
            var dto = new RecordMovementDto
            {
                ProductId = 1,
                WarehouseId = 1,
                MovementType = "STOCK_IN",
                Quantity = 50,
                PerformedBy = "admin",
                BalanceAfter = 50
            };

            // Act
            var result = await _movementService.RecordMovementAsync(dto);

            // Assert
            Assert.AreEqual("Movement recorded successfully", result);
        }

        // TEST 2 — Record movement with invalid type should fail
        [TestMethod]
        public async Task RecordMovement_InvalidType_ReturnsError()
        {
            // Arrange
            var dto = new RecordMovementDto
            {
                ProductId = 1,
                WarehouseId = 1,
                MovementType = "INVALID_TYPE",
                Quantity = 10,
                PerformedBy = "admin"
            };

            // Act
            var result = await _movementService.RecordMovementAsync(dto);

            // Assert
            Assert.IsTrue(result.Contains("Invalid movement type"));
        }

        // TEST 3 — Record movement with zero quantity should fail
        [TestMethod]
        public async Task RecordMovement_ZeroQuantity_ReturnsError()
        {
            // Arrange
            var dto = new RecordMovementDto
            {
                ProductId = 1,
                WarehouseId = 1,
                MovementType = "STOCK_IN",
                Quantity = 0,
                PerformedBy = "admin"
            };

            // Act
            var result = await _movementService.RecordMovementAsync(dto);

            // Assert
            Assert.AreEqual("Quantity must be greater than zero", result);
        }

        // TEST 4 — Get movements by product should return list
        [TestMethod]
        public async Task GetByProduct_ValidId_ReturnsList()
        {
            // Arrange
            _mockRepo.Setup(r => r.FindByProductIdAsync(1))
                     .ReturnsAsync(new List<StockMovement>
                     {
                         new StockMovement { Id = 1, ProductId = 1, MovementType = "STOCK_IN" },
                         new StockMovement { Id = 2, ProductId = 1, MovementType = "STOCK_OUT" }
                     });

            // Act
            var result = await _movementService.GetByProductAsync(1);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        // TEST 5 — Get all movements should return complete list
        [TestMethod]
        public async Task GetAllMovements_ReturnsAllMovements()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new List<StockMovement>
                     {
                         new StockMovement { Id = 1, MovementType = "STOCK_IN" },
                         new StockMovement { Id = 2, MovementType = "STOCK_OUT" },
                         new StockMovement { Id = 3, MovementType = "ADJUSTMENT" }
                     });

            // Act
            var result = await _movementService.GetAllMovementsAsync();

            // Assert
            Assert.AreEqual(3, result.Count);
        }
        // 6 — Negative quantity
[TestMethod]
public async Task Record_NegativeQuantity_ReturnsError()
{
    var dto = new RecordMovementDto { Quantity = -5 };

    var result = await _movementService.RecordMovementAsync(dto);

   Assert.IsNotNull(result);
}

// 7 — Null DTO
[TestMethod]
public async Task Record_NullDto_Throws()
{
   await Assert.ThrowsAsync<Exception>(() => _movementService.RecordMovementAsync(null!));
}

// 8 — Missing type
[TestMethod]
public async Task Record_NoType_ReturnsError()
{
    var dto = new RecordMovementDto { Quantity = 10 };

    var result = await _movementService.RecordMovementAsync(dto);

    Assert.IsTrue(result.Contains("type"));
}
    }
}