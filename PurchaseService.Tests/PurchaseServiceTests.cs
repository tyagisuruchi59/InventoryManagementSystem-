// StockPro Inventory Management System
// Test: Purchase Service | MSTest
// Developer: Suru | April 2026
// Description: Unit tests for PurchaseServiceImpl using Moq

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PurchaseService.DTOs;
using PurchaseService.Models;
using PurchaseService.Publishers;
using PurchaseService.Repositories;
using PurchaseService.Services;
using MassTransit;

namespace PurchaseService.Tests
{
    [TestClass]
    public class PurchaseServiceTests
    {
        private Mock<IPurchaseRepository> _mockRepo;
        private PurchaseServiceImpl _purchaseService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IPurchaseRepository>();
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();
            var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<POApprovedPublisher>>();
            var publisher = new POApprovedPublisher(mockPublishEndpoint.Object, mockLogger.Object);
            _purchaseService = new PurchaseServiceImpl(_mockRepo.Object, publisher);
        }

        // TEST 1 — Create PO should succeed
        [TestMethod]
        public async Task CreatePO_ValidData_ReturnsSuccess()
        {
            var dto = new CreatePODto
            {
                SupplierId = 1,
                WarehouseId = 1,
                CreatedById = 1,
                ExpectedDate = DateTime.UtcNow.AddDays(7),
                ReferenceNumber = "PO-001",
                LineItems = new List<CreateLineItemDto>
                {
                    new CreateLineItemDto
                    {
                        ProductId = 1,
                        Quantity = 10,
                        UnitCost = 100
                    }
                }
            };

            var result = await _purchaseService.CreatePOAsync(dto);

            Assert.AreEqual("Purchase order created successfully", result);
        }

        // TEST 2 — Approve PO should succeed
        [TestMethod]
        public async Task ApprovePO_ValidId_ReturnsSuccess()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new PurchaseOrder
                     {
                         Id = 1,
                         Status = "Draft",
                         LineItems = new List<POLineItem>()
                     });

            var result = await _purchaseService.ApprovePOAsync(1);

            Assert.AreEqual("Purchase order approved successfully", result);
        }

        // TEST 3 — Cancel fully received PO should fail
        [TestMethod]
        public async Task CancelPO_FullyReceived_ReturnsError()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new PurchaseOrder
                     {
                         Id = 1,
                         Status = "FullyReceived",
                         LineItems = new List<POLineItem>()
                     });

            var result = await _purchaseService.CancelPOAsync(1);

            Assert.AreEqual("Cannot cancel a fully received PO", result);
        }

        // TEST 4 — Get PO by invalid ID should return null
        [TestMethod]
        public async Task GetPOById_InvalidId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((PurchaseOrder?)null);

            var result = await _purchaseService.GetPOByIdAsync(999);

            Assert.IsNull(result);
        }

        // TEST 5 — Get POs by status should return filtered list
        [TestMethod]
        public async Task GetPOsByStatus_Draft_ReturnsDraftPOs()
        {
            _mockRepo.Setup(r => r.GetByStatusAsync("Draft"))
                     .ReturnsAsync(new List<PurchaseOrder>
                     {
                         new PurchaseOrder
                         {
                             Id = 1,
                             Status = "Draft",
                             LineItems = new List<POLineItem>()
                         }
                     });

            var result = await _purchaseService.GetPOsByStatusAsync("Draft");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Draft", result[0].Status);
        }
    }
}