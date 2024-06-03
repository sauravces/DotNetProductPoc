using Moq;
using ProductPOC.Models;
using ProductPOC.Service;
using ProductPOC.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ProductPOC.Dto;

namespace ProductTest
{
    public class ProductTestController
    {
        [Fact]
        public async Task GetProducts_Returns_Ok_With_ProductDtos()
        {
            // Arrange
            var products = new List<Product>
              {
                new Product { Id ="665de04a0539588a315a45d1", Name = "Product1", Description = "Description1", Price = 10 },
                new Product { Id ="665de3780539588a315a45d2", Name = "Product2", Description = "Description2", Price = 20 }
              };

            var productServiceMock = new Mock<IProductService>();
            productServiceMock.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);

            var controller = new ProductController(productServiceMock.Object);

            // Act
            var result = await controller.GetProducts();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var productDtos = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;

            productDtos.Should().HaveCount(2);
            productDtos.Should().ContainEquivalentOf(new ProductDto { Id = "665de04a0539588a315a45d1", Name = "Product1", Description = "Description1", Price = 10 });
            productDtos.Should().ContainEquivalentOf(new ProductDto { Id = "665de3780539588a315a45d2", Name = "Product2", Description = "Description2", Price = 20 });
        }
    }
}
