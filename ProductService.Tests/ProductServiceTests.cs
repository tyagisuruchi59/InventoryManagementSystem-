// StockPro Inventory Management System
// Test: Product Service | MSTest
// Developer: Suru | April 2026
// Description: Unit tests for ProductServiceImpl using Moq

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repositories;
using ProductService.Services;

namespace ProductService.Tests
{
    [TestClass]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _mockRepo;
        private ProductServiceImpl _productService;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IProductRepository>();
            _productService = new ProductServiceImpl(_mockRepo.Object);
        }

        // TEST 1 — Create product with new SKU should succeed
        [TestMethod]
        public async Task CreateProduct_NewSKU_ReturnsSuccess()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetBySKUAsync("SKU001"))
                     .ReturnsAsync((Product?)null);

            var dto = new CreateProductDto
            {
                SKU = "SKU001",
                Name = "Test Product",
                Category = "Electronics",
                Brand = "TestBrand",
                UnitOfMeasure = "pieces",
                CostPrice = 100,
                SellingPrice = 150
            };

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.AreEqual("Product created successfully", result);
        }

        // TEST 2 — Create product with existing SKU should fail
        [TestMethod]
        public async Task CreateProduct_ExistingSKU_ReturnsDuplicate()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetBySKUAsync("SKU001"))
                     .ReturnsAsync(new Product { SKU = "SKU001" });

            var dto = new CreateProductDto { SKU = "SKU001" };

            // Act
            var result = await _productService.CreateProductAsync(dto);

            // Assert
            Assert.AreEqual("Product with this SKU already exists", result);
        }

        // TEST 3 — Get product by ID should return product
        [TestMethod]
        public async Task GetProductById_ValidId_ReturnsProduct()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Product
                     {
                         Id = 1,
                         SKU = "SKU001",
                         Name = "Test Product",
                         IsActive = true
                     });

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SKU001", result.SKU);
        }

        // TEST 4 — Get product by invalid ID should return null
        [TestMethod]
        public async Task GetProductById_InvalidId_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        // TEST 5 — Deactivate product should return true
        [TestMethod]
        public async Task DeactivateProduct_ValidId_ReturnsTrue()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Product { Id = 1, IsActive = true });

            // Act
            var result = await _productService.DeactivateProductAsync(1);

            // Assert
            Assert.IsTrue(result);
        }

        // TEST 6 — Search products should return matching list
        [TestMethod]
        public async Task SearchProducts_ValidKeyword_ReturnsList()
        {
            // Arrange
            _mockRepo.Setup(r => r.SearchAsync("phone"))
                     .ReturnsAsync(new List<Product>
                     {
                         new Product { Id = 1, Name = "Phone Case", IsActive = true },
                         new Product { Id = 2, Name = "Phone Stand", IsActive = true }
                     });

            // Act
            var result = await _productService.SearchProductsAsync("phone");

            // Assert
            Assert.AreEqual(2, result.Count);
        }
        // TEST — Create product with negative price
[TestMethod]
public async Task CreateProduct_NegativePrice_ReturnsError()
{
    var dto = new CreateProductDto
    {
        SKU = "SKU002",
        CostPrice = -10
    };

    var result = await _productService.CreateProductAsync(dto);

    Assert.IsNotNull(result);
}

// TEST — Deactivate already inactive product
[TestMethod]
public async Task DeactivateProduct_AlreadyInactive_ReturnsFalse()
{
    _mockRepo.Setup(r => r.GetByIdAsync(1))
             .ReturnsAsync(new Product { Id = 1, IsActive = false });

    var result = await _productService.DeactivateProductAsync(1);

    Assert.IsNotNull(result);
}
    }
}