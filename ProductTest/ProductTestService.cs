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

        [Theory, AutoData]
        public async Task GetAllProducts_ReturnsAllProducts_FromRepository(List<Product> products)
        {
            // Arrange
            _productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);
            // Act
            var result = await _productService.GetAllProductsAsync();
            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Theory, AutoData]
        public async Task GetByIdProduct_ReturnsProduct_FromRepository(Guid productId, Product products)
        {
            // Arrange
            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(products);
            // Act
            var result = await _productService.GetByIdProductAsync(productId);
            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Theory, AutoData]
        public async Task CreateProduct_ShouldReturn_CreatedProduct(Product product)
        {
            // Arrange
            _productRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>()))
                                  .ReturnsAsync(product);
            // Act
            var result = await _productService.CreateProductAsync(product);
            // Assert
            result.Should().BeEquivalentTo(product, options => options.ComparingByMembers<Product>());
            _productRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Theory, AutoData]
        public async Task UpdateProduct_ShouldReturn_UpdatedProduct(Guid productId, Product product, Product existingProduct)
        {
            // Arrange
            _productRepositoryMock.Setup(repo => repo.UpdateAsync(productId, It.IsAny<Product>()))
                                  .ReturnsAsync(product);
            // Act
            var result = await _productService.UpdateProductAsync(productId, product);
            // Assert
            result.Should().BeEquivalentTo(product, options => options.ComparingByMembers<Product>());
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(productId, It.IsAny<Product>()), Times.Once);
        }


        [Theory,AutoData]
        public async Task DeleteByIdProductAsyncShouldReturnDeletedProductWhenProductExists(Guid productId,Product product)
        {
            // Arrange
            _productRepositoryMock.Setup(repo => repo.DeleteByIdAsync(productId)).ReturnsAsync(product);
            // Act
            var result = await _productService.DeleteByIdProductAsync(productId);
            // Assert
            result.Should().BeEquivalentTo(product);
            _productRepositoryMock.Verify(repo => repo.DeleteByIdAsync(productId), Times.Once);
        }

        [Theory,AutoData]
        public async Task DeleteByIdProductAsyncShouldReturnNullWhenProductDoesNotExist(Guid productId)
        {
            // Arrange
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
