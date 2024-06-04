namespace ProductTest
{
    public class ProductTestService
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        public ProductTestService()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
        }
        [Fact]
        public async Task GetAllProductsAsyncReturnsAllProductsFromRepository()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id ="665de04a0539588a315a45d1", Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id ="665de3780539588a315a45d2", Name = "Product2", Description = "Description2", Price = 20 }
            };
            _productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);
            var productService = new ProductService(_productRepositoryMock.Object);
            // Act
            var result = await productService.GetAllProductsAsync();
            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetByIdProductAsyncReturnsProductFromRepository()
        {
            // Arrange
            var productId = "665de04a0539588a315a45d1";
            var products = new Product { Id = "665de04a0539588a315a45d1", Name = "Product1", Description = "Description1", Price = 10 };
            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(products);
            var productService = new ProductService(_productRepositoryMock.Object);
            // Act
            var result = await productService.GetByIdProductAsync(productId);
            // Assert
            result.Should().BeEquivalentTo(products);
        }
    }
}
