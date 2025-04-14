using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using mytown.DataAccess.Interfaces;


namespace MyTown.Controllers
{
    [Route("api/business/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepository productRepo,
                                 ILogger<ProductController> logger)
        {
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpPost("Add_Products")]
        public async Task<IActionResult> CreateProduct([FromBody] products product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            await _productRepo.CreateProductAsync(product);
            return Ok(new { productId = product.product_id });
        }

        [HttpDelete("deleteProduct")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            try
            {
                // Use the repository to delete the product
                await _productRepo.DeleteProductAsync(productId);

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting product: {ex.Message}");

                // Return a generic error response
                return StatusCode(500, new { message = "An error occurred while deleting the product." });
            }
        }

        [HttpPut("updateProduct")]
        public IActionResult UpdateProduct([FromBody] products updatedProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid product data" });
            }

            var isUpdated = _productRepo.UpdateProductAsync(updatedProduct);
            //if (!isUpdated)
            //{
            //    return NotFound(new { message = "Product not found" });
            //}

            return Ok(new { message = "Product updated successfully" });
        }
    }
}