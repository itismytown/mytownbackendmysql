using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.Models;

namespace mytown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository _businessRepository;

        public BusinessController(IBusinessRepository businessRepository)
        {
            _businessRepository = businessRepository;
        }

        [HttpGet("BusinessCategories")]
        public async Task<ActionResult<IEnumerable<businesscategoriescs>>> GetBusinessCategories()
        {
            var categories = await _businessRepository.GetBusinessCategories();
            return Ok(categories);
        }

        [HttpPost("Add_Products")]
        public async Task<IActionResult> CreateProduct([FromBody] products product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            await _businessRepository.CreateProductAsync(product);
            return Ok(new { productId = product.product_id });
        }

        [HttpDelete("deleteProduct")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            try
            {
                // Use the repository to delete the product
                await _businessRepository.DeleteProductAsync(productId);

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

            var isUpdated = _businessRepository.UpdateProduct(updatedProduct);
            if (!isUpdated)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(new { message = "Product updated successfully" });
        }
    }

}
