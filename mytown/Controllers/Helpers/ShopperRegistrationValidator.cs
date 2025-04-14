using System.Collections.Generic;
using System.Text.RegularExpressions;
using mytown.DataAccess.Interfaces;
using mytown.Models;

namespace mytown.Controllers.Helpers
{
    public class ShopperRegistrationValidator : IShopperRegistrationValidator
    {
        public List<string> Validate(ShopperRegisterDto model)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Username))
                errors.Add("Please enter your name.");

            if (string.IsNullOrWhiteSpace(model.Email))
                errors.Add("Email address is required.");
            else
            {
                // Validate email format.
                const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(model.Email, emailPattern))
                    errors.Add("Please enter a valid email address.");
            }

            if (string.IsNullOrWhiteSpace(model.Password))
                errors.Add("Password is required.");
            else
            {
                if (model.Password.Length < 8)
                    errors.Add("Your password must be at least 8 characters long.");

                // Validate password complexity.
                const string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
                if (!Regex.IsMatch(model.Password, passwordPattern))
                    errors.Add("Password must include at least one uppercase letter, one lowercase letter, a number, and a special character.");
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                errors.Add("Phone number is required.");

            // Additional validations (Address, Town, City, etc.) can be added here.

            return errors;
        }
    }
}
