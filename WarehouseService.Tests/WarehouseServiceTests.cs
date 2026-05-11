// StockPro Inventory Management System
// Test: Warehouse Service | MSTest
// Developer: Suru | April 2026
// Description: Unit tests for WarehouseServiceImpl using Moq

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WarehouseService.DTOs;
using WarehouseService.Models;
using WarehouseService.Publishers;
using WarehouseService.Repositories;
using WarehouseService.Services;
using MassTransit;

namespace WarehouseService.Tests
{
    [TestClass]
    public class WarehouseServiceTests
    {
        private Mock<IWarehouseRepository> _mockRepo;
        private WarehouseServiceImpl _warehouseService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IWarehouseRepository>();
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<LowStockPublisher>>();
            var publisher = new LowStockPublisher(mockPublishEndpoint.Object, mockLogger.Object);
            _warehouseService = new WarehouseServiceImpl(_mockRepo.Object, publisher);
        }

        // TEST 1 — Create warehouse should succeed
        [TestMethod]
        public async Task CreateWarehouse_ValidData_ReturnsSuccess()
        {
            var dto = new CreateWarehouseDto
            {
                Name = "Delhi Warehouse",
                Address = "123 Main Street",
                City = "New Delhi",
                Capacity = 1000
            };

            var result = await _warehouseService.CreateWarehouseAsync(dto);

            Assert.AreEqual("Warehouse created successfully", result);
        }

        // TEST 2 — Get warehouse by valid ID should return warehouse
        [TestMethod]
        public async Task GetWarehouseById_ValidId_ReturnsWarehouse()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Warehouse
                     {
                         Id = 1,
                         Name = "Delhi Warehouse",
                         IsActive = true
                     });

            var result = await _warehouseService.GetWarehouseByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Delhi Warehouse", result.Name);
        }

        // TEST 3 — Transfer stock with insufficient quantity should fail
        [TestMethod]
        public async Task TransferStock_InsufficientStock_ReturnsError()
        {
            _mockRepo.Setup(r => r.GetStockLevelAsync(1, 1))
                     .ReturnsAsync(new StockLevel
                     {
                         WarehouseId = 1,
                         ProductId = 1,
                         Quantity = 5,
                         ReservedQuantity = 0
                     });

            var dto = new TransferStockDto
            {
                ProductId = 1,
                FromWarehouseId = 1,
                ToWarehouseId = 2,
                Quantity = 10,
                Reason = "Test",
                TransferredBy = "admin"
            };

            var result = await _warehouseService.TransferStockAsync(dto);

            Assert.AreEqual("Not enough stock in source warehouse", result);
        }

        // TEST 4 — Get low stock items should return list
        [TestMethod]
        public async Task GetLowStock_ReturnsLowStockItems()
        {
            _mockRepo.Setup(r => r.GetLowStockAsync())
                     .ReturnsAsync(new List<StockLevel>
                     {
                         new StockLevel { ProductId = 1, Quantity = 5, ReorderLevel = 10 },
                         new StockLevel { ProductId = 2, Quantity = 2, ReorderLevel = 15 }
                     });

            var result = await _warehouseService.GetLowStockAsync();

            Assert.AreEqual(2, result.Count);
        }

        // TEST 5 — Deactivate warehouse should return true
        [TestMethod]
        public async Task DeactivateWarehouse_ValidId_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Warehouse { Id = 1, IsActive = true });

            var result = await _warehouseService.DeactivateWarehouseAsync(1);

            Assert.IsTrue(result);
        }

        // TEST 6 — Transfer same warehouse
        [TestMethod]
        public async Task Transfer_SameWarehouse_ReturnsError()
        {
            var dto = new TransferStockDto
            {
                FromWarehouseId = 1,
                ToWarehouseId = 1,
                Quantity = 5
            };

            var result = await _warehouseService.TransferStockAsync(dto);

            Assert.IsNotNull(result);
        }

        // TEST 7 — Zero quantity
        [TestMethod]
        public async Task Transfer_ZeroQuantity_ReturnsError()
        {
            var dto = new TransferStockDto { Quantity = 0 };

            var result = await _warehouseService.TransferStockAsync(dto);

            Assert.IsNotNull(result);
        }

        // TEST 8 — Invalid warehouse
        [TestMethod]
        public async Task GetWarehouse_InvalidId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Warehouse?)null);

            var result = await _warehouseService.GetWarehouseByIdAsync(999);

            Assert.IsNull(result);
        }
    }
}