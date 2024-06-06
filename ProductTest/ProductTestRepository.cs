namespace ProductTest
{
    public class ProductTestRepository
    {
        private readonly Mock<IMongoCollection<Product>> _mockCollection;
        private readonly Mock<IAsyncCursor<Product>> _mockCursor;
        private readonly Mock<ProductDbContext> _mockContext;
        private readonly ProductRepository _repository;
        public ProductTestRepository()
        {
            _mockCollection = new Mock<IMongoCollection<Product>>();
            _mockCursor = new Mock<IAsyncCursor<Product>>();
            _mockContext = new Mock<ProductDbContext>();
            _mockContext.Setup(x => x.Products).Returns(_mockCollection.Object);
            _repository = new ProductRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetAllAsyncReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id =Guid.NewGuid(), Name = "Product2", Description = "Description2", Price = 20 }
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
            // Act
            var result = await _repository.GetAllAsync();
            // Assert
            result.Should().BeEquivalentTo(products, options => options.ComparingByMembers<Product>());
        }

        [Fact]
        public async Task GetByIdAsyncReturnsProductWhenIdIsPresent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var expectedProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description1",
                Price = 10
            };
            _mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                       .Returns(true)
                       .Returns(false);
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true)
                    .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(new List<Product> { expectedProduct });
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
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
            var invalidProductId =Guid.NewGuid();
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(default))
                      .ReturnsAsync(false);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                default))
                .ReturnsAsync(_mockCursor.Object);
            _mockContext.Setup(x => x.Products).Returns(_mockCollection.Object);
            // Act
            var result = await _repository.GetByIdAsync(invalidProductId);
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsyncShouldInsertProductAndReturnProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Description = "Description1",
                Price = 10
            };
            _mockCollection.Setup(x => x.InsertOneAsync(product, null, default)).Returns(Task.CompletedTask);
            // Act
            var result = await _repository.CreateAsync(product);
            // Assert
            result.Should().BeEquivalentTo(product);
            _mockCollection.Verify(x => x.InsertOneAsync(product, null, default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShouldReturnUpdatedProductWhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "UpdatedProduct",
                Description = "UpdatedDescription",
                Price = 20
            };
            var existingProduct = new Product
            {
                Id = productId,
                Name = "OldProduct",
                Description = "OldDescription",
                Price = 10
            };
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(default))
                       .ReturnsAsync(true)
                       .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(new List<Product> { existingProduct });
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                default))
                .ReturnsAsync(_mockCursor.Object);
            _mockCollection.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                default))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));
            // Act
            var result = await _repository.UpdateAsync(productId, product);
            // Assert
            result.Should().BeEquivalentTo(product);
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShouldReturnNullWhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "UpdatedProduct",
                Description = "UpdatedDescription",
                Price = 20
            };
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(default))
                       .ReturnsAsync(false);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                default))
                .ReturnsAsync(_mockCursor.Object);
            // Act
            var result = await _repository.UpdateAsync(productId, product);
            // Assert
            result.Should().BeNull();
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                default), Times.Never);
        }

        [Fact]
        public async Task DeleteByIdAsyncShouldReturnDeletedProductWhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var expectedProduct = new Product { Id = productId, Name = "Product1", Description = "Description1", Price = 10 };
            _mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                       .Returns(true)
                       .Returns(false);
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true)
                       .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(new List<Product> { expectedProduct });
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);
            var deleteResult = new DeleteResult.Acknowledged(1);
            _mockCollection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deleteResult);
            // Act
            var result = await _repository.DeleteByIdAsync(productId);
            // Assert
            result.Should().BeEquivalentTo(expectedProduct);
            _mockCollection.Verify(x => x.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockCollection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsyncShouldReturnNullWhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                       .Returns(false);
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);
            // Act
            var result = await _repository.DeleteByIdAsync(productId);
            // Assert
            result.Should().BeNull();
            _mockCollection.Verify(x => x.FindAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<FindOptions<Product, Product>>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockCollection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteAllProducts()
        {
            // Arrange
            var mockDeleteResult = new Mock<DeleteResult>();
            _mockCollection.Setup(x => x.DeleteManyAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockDeleteResult.Object);
            // Act
            await _repository.DeleteAsync();
            // Assert
            _mockCollection.Verify(x => x.DeleteManyAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
