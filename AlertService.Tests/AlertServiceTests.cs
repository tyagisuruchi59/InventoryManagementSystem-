using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AlertService.DTOs;
using AlertService.Models;
using AlertService.Repositories;
using AlertService.Services;
using Microsoft.Extensions.Configuration;

namespace AlertService.Tests
{
    [TestClass]
    public class AlertServiceTests
    {
        private Mock<IAlertRepository> _mockRepo = null!;
        private AlertServiceImpl _alertService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IAlertRepository>();
           var config = new ConfigurationBuilder().Build();
_alertService = new AlertServiceImpl(_mockRepo.Object, config);
        }

        // 1
        [TestMethod]
        public async Task SendAlert_ValidData_ReturnsSuccess()
        {
            var dto = new CreateAlertDto
            {
                RecipientId = 1,
                Type = "LOW_STOCK",
                Severity = "CRITICAL",
                Title = "Low Stock",
                Message = "Below level"
            };

            var result = await _alertService.SendAlertAsync(dto);

            Assert.AreEqual("Alert sent successfully", result);
        }

        // 2
        [TestMethod]
        public async Task SendAlert_EmptyMessage_ReturnsError()
        {
            var dto = new CreateAlertDto
            {
                RecipientId = 1,
                Message = ""
            };

            var result = await _alertService.SendAlertAsync(dto);

            Assert.IsNotNull(result);
        }

        // 3
        [TestMethod]
        public async Task SendBulkAlert_Valid_ReturnsSuccess()
        {
            var dto = new BulkAlertDto
            {
                RecipientIds = new List<int> { 1, 2 },
                Type = "SYSTEM",
                Severity = "INFO",
                Title = "Update",
                Message = "Maintenance"
            };

            var result = await _alertService.SendBulkAlertAsync(dto);

            Assert.AreEqual("Bulk alert sent to 2 recipients", result);
        }

        // 4
        [TestMethod]
        public async Task SendBulkAlert_EmptyList_ReturnsError()
        {
            var dto = new BulkAlertDto
            {
                RecipientIds = new List<int>(),
                Message = "Test"
            };

            var result = await _alertService.SendBulkAlertAsync(dto);

            Assert.IsTrue(result.Contains("recipient"));
        }

        // 5
        [TestMethod]
        public async Task MarkAsRead_ValidId_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Alert { Id = 1, IsRead = false });

            var result = await _alertService.MarkAsReadAsync(1);

            Assert.IsTrue(result);
        }

        // 6
        [TestMethod]
        public async Task MarkAsRead_InvalidId_ReturnsFalse()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99))
                     .ReturnsAsync((Alert?)null);

            var result = await _alertService.MarkAsReadAsync(99);

            Assert.IsFalse(result);
        }

        // 7
        [TestMethod]
        public async Task Acknowledge_ValidId_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Alert { Id = 1, IsAcknowledged = false });

            var result = await _alertService.AcknowledgeAsync(1);

            Assert.IsTrue(result);
        }

        // 8
        [TestMethod]
        public async Task Acknowledge_InvalidId_ReturnsFalse()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(50))
                     .ReturnsAsync((Alert?)null);

            var result = await _alertService.AcknowledgeAsync(50);

            Assert.IsFalse(result);
        }

        // 9
        [TestMethod]
        public async Task GetAll_ReturnsList()
        {
            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new List<Alert>
                     {
                         new Alert { Id = 1 },
                         new Alert { Id = 2 }
                     });

            var result = await _alertService.GetAllAsync();

            Assert.AreEqual(2, result.Count);
        }

        // 10
        [TestMethod]
        public async Task GetAll_Empty_ReturnsZero()
        {
            _mockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new List<Alert>());

            var result = await _alertService.GetAllAsync();

            Assert.AreEqual(0, result.Count);
        }

        // 11
        [TestMethod]
        public async Task MarkAsRead_AlreadyRead_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Alert { Id = 1, IsRead = true });

            var result = await _alertService.MarkAsReadAsync(1);

            Assert.IsTrue(result);
        }
    }
}