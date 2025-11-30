using Contacts.Interfaces;
using Contacts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Contacts.Services
{
    public class AuthService : IAuthService
    {
        private readonly ContactContext _dbContext;
        private readonly PasswordHasher<User> _hasher = new();
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor that injects the database context and configuration dependencies.
        /// </summary>
        /// <param name="contactContext"></param>
        /// <param name="config"></param>
        public AuthService(ContactContext contactContext, IConfiguration config)
        {
            _dbContext = contactContext;
            _config = config;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> Login(LoginModel model)
        {
            var user = await _dbContext.Users.Where(u => u.Username == model.Username).SingleOrDefaultAsync();

            if (user == null)
                return string.Empty;


            var verifyHash = _hasher.VerifyHashedPassword(user, user.PwdHash, model.Password);

            if (verifyHash == PasswordVerificationResult.Success)
            {
                return await GenerateToken(model.Username);
            }

            return string.Empty;
        }

        /// <summary>
        /// Registers a new user and contact in the database.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<bool> Register(RegisterModel contact)
        {
			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try {
                transaction.CreateSavepoint("BeforeRegisterNewUser");

                int subcategoryId = 0;
                if (!string.IsNullOrEmpty(contact.SubcategoryName)) {
                    subcategoryId = _dbContext.Subcategories
                        .Where(s => s.Name == contact.SubcategoryName && s.CategoryId == contact.CategoryId)
                        .Select(s => s.Id)
                        .SingleOrDefault();
                    if (subcategoryId == 0) {
						var subcategory = new Subcategory {
							CategoryId = contact.CategoryId,
							Name = contact.SubcategoryName
						};
						_dbContext.Subcategories.Add(subcategory);
						await _dbContext.SaveChangesAsync();
						subcategoryId = subcategory.Id;
					}
                }

				Contact newContact = new Contact {
                    Id = Guid.NewGuid(),
                    FirstName = contact.FirstName,
                    Surname = contact.Surname,
                    Email = contact.Email,
                    Category = _dbContext.Categories.Where(c => c.Id == contact.CategoryId).SingleOrDefault(),
                    BirthDate = contact.DateOfBirth.HasValue ? DateOnly.FromDateTime(contact.DateOfBirth.Value) : null,
                    SubcategoryId = subcategoryId != 0 ? subcategoryId : null,
                    Phone = contact.Phone
				};

                User newUser = new User {
                    Id = Guid.NewGuid(),
                    ContactId = newContact.Id,
                    Username = contact.Email,
                };

                newUser.PwdHash = _hasher.HashPassword(newUser, contact.Password);

                _dbContext.Contacts.Add(newContact);
                _dbContext.Users.Add(newUser);
                await transaction.CommitAsync();

                if (await _dbContext.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            } catch (Exception) {
                transaction.RollbackToSavepoint("BeforeRegisterNewUser");
                return false;
            }
        }

        /// <summary>
        /// Generates a JWT token for the specified email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<string> GenerateToken(string email)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
