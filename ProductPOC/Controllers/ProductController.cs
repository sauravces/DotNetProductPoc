using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductPOC.Dto;
using ProductPOC.Models;
using ProductPOC.Service;

namespace ProductPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDtos= _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByIdProduct([FromRoute] Guid id)
        {
            var product = await _productService.GetByIdProductAsync(id);
            var productDtos = _mapper.Map<ProductDto>(product);
            return Ok(productDtos);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var product= _mapper.Map<Product>(createProductDto);
            product= await _productService.CreateProductAsync(product);
            var productDto= _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetByIdProduct), new { id = productDto.Id }, productDto);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductDto updateProductDto)
        {
            var product = _mapper.Map<Product>(updateProductDto);
            product = await _productService.UpdateProductAsync(id, product);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return AcceptedAtAction(nameof(GetByIdProduct), new { id = productDto.Id }, productDto);
        }
    }
}
