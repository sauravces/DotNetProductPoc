using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using ProductPOC.DbContext;
using ProductPOC.Models;
using ProductPOC.Repository;
using Xunit;

namespace ProductTest
{
    public class ProductTestRepository
    {
        [Fact]
        public async Task GetAllAsync_Returns_All_Products()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = "1", Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id = "2", Name = "Product2", Description = "Description2", Price = 20 }
            };

            // Mock IMongoCollection<Product>
            var mockCursor = new Mock<IAsyncCursor<Product>>();
            mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            mockCursor.Setup(_ => _.Current).Returns(products);

            var mockCollection = new Mock<IMongoCollection<Product>>();
            mockCollection.Setup(x => x.FindSync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), default))
                          .Returns(mockCursor.Object);

            // Mock ProductDbContext and set up its Products property to return the mock IMongoCollection<Product>
            var mockContext = new Mock<ProductDbContext>();
            mockContext.Setup(x => x.Products).Returns(mockCollection.Object);

            // Create an instance of ProductRepository with the mocked ProductDbContext
            var repository = new ProductRepository(mockContext.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(products);
        }
    }
}
