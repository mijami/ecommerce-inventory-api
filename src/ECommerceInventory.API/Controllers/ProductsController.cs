using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceInventory.Application.DTOs;
using ECommerceInventory.Application.Services;

namespace ECommerceInventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Products")]
public class ProductsController : BaseController
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products with optional filtering and pagination
    /// </summary>
    /// <param name="categoryId">Filter by category ID</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="page">Page number for pagination</param>
    /// <param name="limit">Number of items per page</param>
    /// <returns>List of products</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDto>))]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        try
        {
            var hasFilters = categoryId.HasValue || minPrice.HasValue || maxPrice.HasValue;
            var hasExplicitPagination = page > 1
                || HttpContext.Request.Query.ContainsKey(nameof(page))
                || HttpContext.Request.Query.ContainsKey(nameof(limit));

            if (hasFilters || hasExplicitPagination)
            {
                var query = new ProductQueryDto
                {
                    CategoryId = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Page = page,
                    Limit = limit
                };

                var filteredProducts = await _productService.GetFilteredProductsAsync(query);
                return Ok(filteredProducts);
            }

            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "retrieving products");
        }
    }

    /// <summary>
    /// Search products by name or description
    /// </summary>
    /// <param name="q">Search term</param>
    /// <returns>List of matching products</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchProducts([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Search term is required" });
        }

        try
        {
            var products = await _productService.SearchProductsAsync(q);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "searching products");
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(product);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "retrieving the product");
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="createProductDto">Product creation details</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            var product = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "creating the product");
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updateProductDto">Product update details</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, updateProductDto);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "updating the product");
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Product not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "deleting the product");
        }
    }
}