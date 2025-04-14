using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.DataAccess.Interfaces;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    //public class ProductRepository : IProductRepository
    //      private readonly AppDbContext _context;

    //public ProductRepository(AppDbContext context)
    //{
    //    _context = context;
    //}


    //public async Task<products> CreateProductAsync(products product)
    //{
    //    await _context.products.AddAsync(product);
    //    await _context.SaveChangesAsync();
    //    return product;
    //}
    
    //    //edit or update product details 
    //   public async Task DeleteProductAsync(int productId)
    //{
    //    var product = await _context.products
    //        .FirstOrDefaultAsync(p => p.product_id == productId);
    //    if (product != null)
    //    {
    //        _context.products.Remove(product);
    //        await _context.SaveChangesAsync();
    //    }
    //}

    //public async Task UpdateProductAsync(products product)
    //{
    //    _context.products.Update(product);
    //    await _context.SaveChangesAsync();


    //}
}

