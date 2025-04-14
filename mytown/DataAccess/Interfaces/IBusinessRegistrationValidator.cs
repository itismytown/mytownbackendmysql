using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessRegistrationValidator
    {
        /// <summary>
        /// Validates the business registration DTO.
        /// </summary>
        /// <param name="model">The registration DTO.</param>
        /// <returns>A list of validation error messages, if any.</returns>
        List<string> Validate(BusinessRegisterDto model);
    }
}
