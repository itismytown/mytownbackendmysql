﻿using Microsoft.EntityFrameworkCore;
using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface ICartRepository
    {
        Task<addtocart> AddToCart(addtocart cartItem);

        Task<IEnumerable<CartItemDto>> GetCartItems(int shopperRegId);

        Task<bool> RemoveFromCart(int cartId);

        Task<bool> DecreaseCartItemQty(int cartId);

        Task<bool> IncreaseCartItemQty(int cartId);

        Task<bool> MoveToWishlist(int cartId);

        Task<bool> MoveBackToCart(int cartId);

        Task<bool> UpdateCartStatusAsync(int orderId);

        Task<bool> UpdateCartStatusByShopperAsync(int shopperRegId);
        Task<ShopperRegister> GetShopperDetails(int shopperRegId);
        

    }
}

  