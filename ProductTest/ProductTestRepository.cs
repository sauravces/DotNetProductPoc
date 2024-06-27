namespace ProductTest
{
    public class ProductTestRepository
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMongoCollection<Product>> _mockCollection;
        private readonly Mock<IAsyncCursor<Product>> _mockCursor;
        private readonly Mock<ProductDbContext> _mockContext;
        private readonly ProductRepository _repository;

        public ProductTestRepository()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockCollection = _fixture.Freeze<Mock<IMongoCollection<Product>>>();
            _mockCursor = _fixture.Freeze<Mock<IAsyncCursor<Product>>>();
            _mockContext = _fixture.Freeze<Mock<ProductDbContext>>();
            _mockContext.Setup(x => x.Products).Returns(_mockCollection.Object);
            _repository = new ProductRepository(_mockContext.Object);
        }

        [Theory, AutoData]
        public async Task GetAll_ReturnsAll_Products(List<Product> products)
        {
            // Arrange
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(products);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(products, options => options.ComparingByMembers<Product>());
        }

        [Theory, AutoData]
        public async Task GetById_Returns_Product(Guid productId, Product expectedProduct)
        {
            // Arrange
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true)
                       .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(new List<Product> { expectedProduct });
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(productId);

            // Assert
            result.Should().BeEquivalentTo(expectedProduct, options => options.ComparingByMembers<Product>());
        }

        [Theory, AutoData]
        public async Task GetById_ShouldReturnNull_WhenInvalidId(Guid invalidProductId)
        {
            // Arrange
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.GetByIdAsync(invalidProductId);

            // Assert
            result.Should().BeNull();
        }

        [Theory, AutoData]
        public async Task Create_ShouldInsertProduct_AndReturnProduct(Product product)
        {
            // Arrange
            _mockCollection.Setup(x => x.InsertOneAsync(product, null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateAsync(product);

            // Assert
            result.Should().BeEquivalentTo(product);
            _mockCollection.Verify(x => x.InsertOneAsync(product, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task Update_ShouldReturn_UpdatedProduct(Guid productId, Product product, Product existingProduct)
        {
            // Arrange
            product.Id = productId; // Ensure the product has the correct Id
            existingProduct.Id = productId; // Ensure the existing product has the correct Id

            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true)
                       .ReturnsAsync(false);
            _mockCursor.Setup(_ => _.Current).Returns(new List<Product> { existingProduct });
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);
            _mockCollection.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, null));

            // Act
            var result = await _repository.UpdateAsync(productId, product);

            // Assert
            result.Should().BeEquivalentTo(product);
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }


        [Theory, AutoData]
        public async Task Update_ShouldReturnNull_WhenProductNotFound(Guid productId, Product product)
        {
            // Arrange
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            // Act
            var result = await _repository.UpdateAsync(productId, product);

            // Assert
            result.Should().BeNull();
            _mockCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<Product>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory,AutoData]
        public async Task DeleteByIdAsyncShouldReturnDeletedProductWhenProductExists(Guid productId,Product expectedProduct)
        {
            // Arrange
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

        [Theory, AutoData]
        public async Task DeleteByIdAsyncShouldReturnNullWhenProductDoesNotExist(Guid productId)
        {
            // Arrange
            _mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                       .Returns(false);
            _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);
            _mockCollection.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);
            _mockCollection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeleteResult)null);

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
