namespace ProductTest
{
    public class ProductTestController
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductController _controller;

        public ProductTestController()
        {
            _productServiceMock = new Mock<IProductService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ProductController(_productServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetProductsReturnsOkWithProductDtos()
        {
            // Arrange
            var products = new List<Product>
              {
                new Product { Id ="665de04a0539588a315a45d1", Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id ="665de3780539588a315a45d2", Name = "Product2", Description = "Description2", Price = 20 }
              };
            var productDtos = new List<ProductDto>
              {
                new ProductDto { Id ="665de04a0539588a315a45d1", Name = "Product1", Description = "Description1", Price = 10 },
                new ProductDto { Id ="665de3780539588a315a45d2", Name = "Product2", Description = "Description2", Price = 20 }
              };
            _productServiceMock.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>())).Returns(productDtos);
            var controller = new ProductController(_productServiceMock.Object, _mapperMock.Object);
            // Act
            var result = await controller.GetProducts();
            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedProductDtos = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
            returnedProductDtos.Should().HaveCount(2);
            returnedProductDtos.Should().ContainEquivalentOf(productDtos[0]);
            returnedProductDtos.Should().ContainEquivalentOf(productDtos[1]);
        }

        [Fact]
        public async Task GetProductByIdReturnsProductWithProductDtos()
        {
            // Arrange
            var productId = "665de04a0539588a315a45d1";
            var product = new Product { Id = productId, Name = "Product1", Description = "Description1", Price = 10 };
            var productDto = new ProductDto { Id = productId, Name = "Product1", Description = "Description1", Price = 10 };
            _productServiceMock.Setup(x => x.GetByIdProductAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDto);
            var controller = new ProductController(_productServiceMock.Object, _mapperMock.Object);
            // Act
            var result = await controller.GetByIdProductAsync(productId);
            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(productDto, okResult.Value);
        }

    }
}
