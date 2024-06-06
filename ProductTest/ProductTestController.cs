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
                new Product { Id =Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id =Guid.NewGuid(), Name = "Product2", Description = "Description2", Price = 20 }
              };
            var productDtos = new List<ProductDto>
              {
                new ProductDto { Id =Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 },
                new ProductDto { Id =Guid.NewGuid(), Name = "Product2", Description = "Description2", Price = 20 }
              };
            _productServiceMock.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>())).Returns(productDtos);
            // Act
            var result = await _controller.GetProducts();
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
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product1", Description = "Description1", Price = 10 };
            var productDto = new ProductDto { Id = productId, Name = "Product1", Description = "Description1", Price = 10 };
            _productServiceMock.Setup(x => x.GetByIdProductAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDto);
            // Act
            var result = await _controller.GetByIdProductAsync(productId);
            var okResult = result.Result as OkObjectResult;
            // Assert
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(productDto);
        }

        [Fact]
        public async Task CreateProductReturnsProductWithOkResult()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 };
            var productDto = new ProductDto { Id = Guid.NewGuid(), Name = "Product1", Description = "Description1", Price = 10 };
            var createProductDto = new CreateProductDto {  Name = "Product1", Description = "Description1", Price = 10 };
            _mapperMock.Setup(m=>m.Map<Product>(createProductDto)).Returns(product);
            _productServiceMock.Setup(x => x.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);
            // Act
            var result = await _controller.CreateProductAsync(createProductDto) as OkObjectResult;
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.StatusCode.Should().Be(200);
            var okResult=result.As<OkObjectResult>();
            okResult.Value.Should().BeEquivalentTo(productDto,options=>options.ComparingByMembers<ProductDto>());
        }

        [Fact]
        public async Task UpdateProductAsyncShouldReturnUpdatedProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateProductDto = new UpdateProductDto
            {
                Name = "UpdatedProduct1",
                Description = "UpdatedDescription1",
                Price = 15
            };
            var product = new Product
            {
                Id = productId,
                Name = updateProductDto.Name,
                Description = updateProductDto.Description,
                Price = updateProductDto.Price
            };
            var updatedProduct = new Product
            {
                Id = productId,
                Name = "UpdatedProduct1",
                Description = "UpdatedDescription1",
                Price = 15
            };
            var updatedProductDto = new ProductDto
            {
                Id = productId,
                Name = "UpdatedProduct1",
                Description = "UpdatedDescription1",
                Price = 15
            };
            _mapperMock.Setup(m => m.Map<Product>(updateProductDto)).Returns(product);
            _productServiceMock.Setup(x => x.UpdateProductAsync(productId, product)).ReturnsAsync(updatedProduct);
            _mapperMock.Setup(m => m.Map<ProductDto>(updatedProduct)).Returns(updatedProductDto);
            // Act
            var result = await _controller.UpdateProductAsync(productId, updateProductDto);
            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(updatedProductDto);
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

        [Fact]
        public async Task DeleteByIdProductAsyncReturnsNotFoundWhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productServiceMock.Setup(x => x.DeleteByIdProductAsync(productId)).ReturnsAsync((Product)null);
            // Act
            var result = await _controller.DeleteByIdProductAsync(productId) as NotFoundResult;
            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAllProductsAsyncReturnsNoContent()
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
