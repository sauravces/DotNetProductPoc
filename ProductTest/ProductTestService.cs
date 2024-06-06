namespace ProductTest
{
    public class ProductTestService
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly ProductService _productService;
        public ProductTestService()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }
        [Fact]
        public async Task GetAllProductsAsyncReturnsAllProductsFromRepository()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id =Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id =Guid.NewGuid(), Name = "Product2", Description = "Description2", Price = 20 }
            };
            _productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);
            // Act
            var result = await _productService.GetAllProductsAsync();
            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetByIdProductAsyncReturnsProductFromRepository()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var products = new Product { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 };
            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(products);
            // Act
            var result = await _productService.GetByIdProductAsync(productId);
            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldReturnCreatedProduct()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 };
            _productRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>()))
                                  .ReturnsAsync(product);
            // Act
            var result = await _productService.CreateProductAsync(product);
            // Assert
            result.Should().BeEquivalentTo(product, options => options.ComparingByMembers<Product>());
            _productRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldReturnUpdatedProduct()
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
            _productRepositoryMock.Setup(repo => repo.UpdateAsync(productId, It.IsAny<Product>()))
                                  .ReturnsAsync(product);
            // Act
            var result = await _productService.UpdateProductAsync(productId, product);
            // Assert
            result.Should().BeEquivalentTo(product, options => options.ComparingByMembers<Product>());
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(productId, It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdProductAsyncShouldReturnDeletedProductWhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product1", Description = "Description1", Price = 10 };
            _productRepositoryMock.Setup(repo => repo.DeleteByIdAsync(productId)).ReturnsAsync(product);
            // Act
            var result = await _productService.DeleteByIdProductAsync(productId);
            // Assert
            result.Should().BeEquivalentTo(product);
            _productRepositoryMock.Verify(repo => repo.DeleteByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdProductAsyncShouldReturnNullWhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(repo => repo.DeleteByIdAsync(productId)).ReturnsAsync((Product)null);
            // Act
            var result = await _productService.DeleteByIdProductAsync(productId);
            // Assert
            result.Should().BeNull();
            _productRepositoryMock.Verify(repo => repo.DeleteByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteAllProductAsyncShouldDeleteAllProducts()
        {
            // Arrange
            _productRepositoryMock.Setup(repo => repo.DeleteAsync()).Returns(Task.CompletedTask);
            // Act
            await _productService.DeleleAllProductAsync();
            // Assert
            _productRepositoryMock.Verify(repo => repo.DeleteAsync(), Times.Once);
        }

    }
}
