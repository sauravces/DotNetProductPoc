using Microsoft.AspNetCore.Http;

namespace ProductTest
{
    public class ProductTestController
    {
        private readonly IFixture _fixture;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductController _controller;

        public ProductTestController()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _productServiceMock = new Mock<IProductService>();
           _mapperMock = new Mock<IMapper>();
            _controller = new ProductController(_productServiceMock.Object, _mapperMock.Object);
        }

        [Theory]
        [AutoData]
        public async Task GetProducts_ReturnsOk_WithProductDtos(
            List<Product> products,
            List<ProductDto> productDtos)
        {
            // Arrange
            _productServiceMock.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>())).Returns(productDtos);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedProductDtos = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
            returnedProductDtos.Should().HaveCount(productDtos.Count);
            returnedProductDtos.Should().BeEquivalentTo(productDtos);
        }

        [Theory]
        [AutoData]
        public async Task GetProductById_ReturnsProduct_WithProductDtos(
            Guid productId,
            Product product,
            ProductDto productDto)
        {
            // Arrange
            product.Id = productId;
            _productServiceMock.Setup(x => x.GetByIdProductAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDto);

            // Act
            var result = await _controller.GetByIdProduct(productId);
            var okResult = result.Result as OkObjectResult;

            // Assert
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(productDto);
        }

        [Theory]
        [AutoData]
        public async Task CreateProduct_ShouldReturnCreatedAtAction_WithProductDto(
            CreateProductDto createProductDto,
            Product product,
            ProductDto productDto)
        {
            // Arrange
            product.Id = Guid.NewGuid();
            _mapperMock.Setup(m => m.Map<Product>(createProductDto)).Returns(product);
            _productServiceMock.Setup(s => s.CreateProductAsync(product)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _controller.CreateProduct(createProductDto);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(ProductController.GetByIdProduct));
            createdAtActionResult.RouteValues["id"].Should().Be(productDto.Id);
            createdAtActionResult.Value.Should().BeEquivalentTo(productDto);
        }

        [Theory]
        [AutoData]
        public async Task UpdateProduct_ShouldReturnAcceptedAtAction_WithProductDto(
           Guid productId,
           UpdateProductDto updateProductDto,
           Product product,
           ProductDto productDto)
        {
            // Arrange
            product.Id = productId;
            _mapperMock.Setup(m => m.Map<Product>(updateProductDto)).Returns(product);
            _productServiceMock.Setup(x => x.UpdateProductAsync(productId, product)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _controller.UpdateProduct(productId, updateProductDto);

            // Assert
            var acceptedAtActionResult = result.Should().BeOfType<AcceptedAtActionResult>().Subject;
            acceptedAtActionResult.ActionName.Should().Be(nameof(ProductController.GetByIdProduct));
            acceptedAtActionResult.RouteValues["id"].Should().Be(productDto.Id);
            acceptedAtActionResult.Value.Should().BeEquivalentTo(productDto);
            acceptedAtActionResult.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        [Theory]
        [AutoData]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenProductNotFound(
            Guid productId,
            UpdateProductDto updateProductDto)
        {
            // Arrange
            _productServiceMock.Setup(x => x.UpdateProductAsync(productId, It.IsAny<Product>())).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateProduct(productId, updateProductDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }


        [Fact]
        public async Task DeleteByIdProductAsyncReturnsOkWhenProductIsDeleted()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product1", Description = "Description1", Price = 10 };
            _productServiceMock.Setup(x => x.DeleteByIdProductAsync(productId)).ReturnsAsync(product);
            // Act
            var result = await _controller.DeleteByIdProductAsync(productId) as OkObjectResult;
            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(product);
        }

        [Theory ,AutoData]
        public async Task DeleteByIdProduct_ReturnsNotFound_WhenProductDoesNotExist(Guid productId)
        {
            // Arrange
            _productServiceMock.Setup(x => x.DeleteByIdProductAsync(productId)).ReturnsAsync((Product)null);
            // Act
            var result = await _controller.DeleteByIdProductAsync(productId) as NotFoundResult;
            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAllProducts_Returns_NoContent()
        {
            // Arrange
            _productServiceMock.Setup(service => service.DeleleAllProductAsync())
                        .Returns(Task.CompletedTask);
            // Act
            var result = await _controller.DeleteAllProductAsync();
            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
