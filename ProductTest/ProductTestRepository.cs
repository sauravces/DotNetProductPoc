namespace ProductTest
{
    public class ProductTestRepository
    {
        private readonly Mock<IMongoCollection<Product>> _mockCollection;
        private readonly Mock<IAsyncCursor<Product>> _mockCursor;
        private readonly Mock<ProductDbContext> _mockContext;

        public ProductTestRepository()
        {
            _mockCollection = new Mock<IMongoCollection<Product>>();
            _mockCursor = new Mock<IAsyncCursor<Product>>();
            _mockContext = new Mock<ProductDbContext>();
        }

        [Fact]
        public async Task GetAllAsyncReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = "1", Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id = "2", Name = "Product2", Description = "Description2", Price = 20 }
            };
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(default))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(products);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                default))
                .ReturnsAsync(_mockCursor.Object);
            _mockContext.Setup(x => x.Products).Returns(_mockCollection.Object);
            var repository = new ProductRepository(_mockContext.Object);
            // Act
            var result = await repository.GetAllAsync();
            // Assert
            result.Should().BeEquivalentTo(products, options => options.ComparingByMembers<Product>());
        }

        [Fact]
        public async Task GetByIdAsyncReturnsProduct()
        {
            // Arrange
            var productId = "665de04a0539588a315a45d1";
            var expectedProduct = new Product
            {
                Id = "665de04a0539588a315a45d1",
                Name = "Product1",
                Description = "Description1",
                Price = 10
            };
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(default))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(() => expectedProduct);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                default))
                .ReturnsAsync(_mockCursor.Object);
            _mockContext.Setup(x => x.Products).Returns(_mockCollection.Object);
            var repository = new ProductRepository(_mockContext.Object);
            // Act
            var result = await repository.GetByIdAsync(productId);
            // Assert
            result.Should().BeEquivalentTo(expectedProduct, options => options.ComparingByMembers<Product>());
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnNullWhenInvalidId()
        {
            // Arrange
            var invalidProductId = "invalid_id";
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(default))
                      .ReturnsAsync(false);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                default))
                .ReturnsAsync(_mockCursor.Object);
            _mockContext.Setup(x => x.Products).Returns(_mockCollection.Object);
            var repository = new ProductRepository(_mockContext.Object);
            // Act
            var result = await repository.GetByIdAsync(invalidProductId);
            // Assert
            result.Should().BeNull();
        }
    }
}
