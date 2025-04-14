using System.Collections.Generic;
using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IShopperRegistrationValidator
    {
        /// <summary>
        /// Validates the shopper registration DTO.
        /// </summary>
        /// <param name="model">The registration DTO.</param>
        /// <returns>A list of validation error messages, if any.</returns>
        List<string> Validate(ShopperRegisterDto model);
    }
}
