using Contacts.Models;

namespace Contacts.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> Login(LoginModel model);

        /// <summary>
        /// Registers a new user and contact in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> Register(RegisterModel model);
    }
}
