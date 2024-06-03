using FluentAssertions;
using Moq;
using ProductPOC.Models;
using ProductPOC.Repository;
using ProductPOC.Service;

namespace ProductTest
{
    public class ProductTestService
    {
        [Fact]
        public async Task GetAllProductsAsync_Returns_All_Products_From_Repository()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id ="665de04a0539588a315a45d1", Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id ="665de3780539588a315a45d2", Name = "Product2", Description = "Description2", Price = 20 }
            };
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);
            var productService = new ProductService(productRepositoryMock.Object);
            // Act
            var result = await productService.GetAllProductsAsync();
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(products);
        }
    }
}
