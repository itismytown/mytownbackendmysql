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

        // GET: api/products/{id}
        [HttpGet("GetProductById/{productId}")]
        public async Task<ActionResult<products>> GetProductById(int productId)
        {
            var product = await _productRepo.GetProductById(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: api/User/GetAllProducts
        [HttpGet("GetAllProductsforbusid/{BusRegId}")]
        public async Task<ActionResult<products>> GetAllProducts(int BusRegId)
        {
            try
            {
                // Fetch all products from the repository
                var products = await _productRepo.GetAllProductsAsync(BusRegId);

                // Check if no products were found
                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                // Return the list of products with a 200 OK status
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Handle any errors and return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetDiscountedProductsAsync")]
        public async Task<ActionResult<products>> GetDiscountedProductsAsync()
        {
            try
            {
                // Fetch all products from the repository
                var products = await _productRepo.GetDiscountedProductsAsync();

                // Check if no products were found
                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                // Return the list of products with a 200 OK status
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Handle any errors and return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetProductsBySubCategory/{subCategoryId}")]
        public async Task<IActionResult> GetProductsBySubCategory(int subCategoryId)
        {
            var products = await _productRepo.GetProductsBySubCategoryAsync(subCategoryId);
            if (products == null || !products.Any())
            {
                return NotFound(new { Message = "No products found for this subcategory" });
            }

            return Ok(products);
        }

        //save shopper recently viewd product
        [HttpPost("ShopperRecentViewProduct")]
        public async Task<IActionResult> ShopperRecentViewProduct(int shopperId, int productId)
        {
            await _productRepo.SaveProductViewAsync(shopperId, productId);
            return Ok(new { message = "Product view recorded" });
        }
    }
}