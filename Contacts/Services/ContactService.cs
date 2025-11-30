using Contacts.Interfaces;
using Contacts.Models;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Services
{
    public class ContactService : IContactService {
        private readonly ContactContext _dbContext;

        /// <summary>
        /// Constructor that injects the database context dependency.
        /// </summary>
        /// <param name="contactContext"></param>
        public ContactService(ContactContext contactContext) {
            _dbContext = contactContext;
        }

        /// <summary>
        /// Deletes a contact and its related users by contact ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Guid id) {
            var c = await _dbContext.Contacts.FirstAsync(c => c.Id == id);
            if (c != null) {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try {
                    transaction.CreateSavepoint("BeforeDeleteContact");
                    _dbContext.Users.Where(u => u.ContactId == c.Id)
                        .ExecuteDeleteAsync().Wait();
                    await _dbContext.SaveChangesAsync();

                    _dbContext.Contacts.Remove(c);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return true;
                } catch {
                    await transaction.RollbackToSavepointAsync("BeforeDeleteContact");
                    return false;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// Retrieves a contact by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Contact> Get(Guid id) {
            return await _dbContext.Contacts.FirstAsync(c => c.Id == id);
        }

        /// <summary>
        /// Retrieves all contacts ordered by surname.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Contact>> GetAll() {
            return await _dbContext.Contacts.OrderBy(s => s.Surname).ToListAsync();
        }

        /// <summary>
        /// Updates a contact with new data and handles subcategory creation if needed.
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<bool> Update(Guid contactId, ContactDTO contact) {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var c = await _dbContext.Contacts.FirstAsync(c => c.Id == contactId);
            var oldBirthDate = c?.BirthDate.HasValue == true ? c.BirthDate.Value.ToString("yyyy-MM-dd") : "";
            var category = await _dbContext.Categories.FirstOrDefaultAsync(cat => cat.Id == contact.CategoryId);

			int subcategoryId = 0;
            Subcategory subcategory;

            transaction.CreateSavepoint("BeforeUpdateContact");

            try {
               
                var oldSubcategory = await _dbContext.Subcategories.FirstOrDefaultAsync(s => s.Id == c.SubcategoryId) ?? new();
                var oldVersion = new ContactDTO(c.Id, c.FirstName, c.Surname, c.Email, c.Phone, oldBirthDate, c.CategoryId, oldSubcategory.Name);
                if (oldVersion.Equals(contact)) {
                    return true;
                }

				if (!string.IsNullOrEmpty(contact.SubcategoryName)) {
					subcategory = await _dbContext.Subcategories.FirstOrDefaultAsync(s => s.CategoryId == contact.CategoryId && s.Name.Trim().ToLower() == contact.SubcategoryName.Trim().ToLower());
                    if(subcategory.Id > 0) {
                        subcategoryId = subcategory.Id;
					}
					if (subcategory == null) {
						subcategory = new Subcategory {
							CategoryId = contact.CategoryId,
							Name = contact.SubcategoryName
						};
						_dbContext.Subcategories.Add(subcategory);
						await _dbContext.SaveChangesAsync();
						subcategoryId = subcategory.Id;
					}
				}

				if (c != null) {
                    c.FirstName = contact.FirstName;
                    c.Surname = contact.Surname;
                    c.Email = contact.Email;
                    c.Phone = contact.Phone;
                    if (DateOnly.TryParse(contact.DateOfBirth, out var dob)) {
                        c.BirthDate = dob;
                    } else {
                        c.BirthDate = null;
                    }
                    c.CategoryId = contact.CategoryId;
                    c.SubcategoryId = subcategoryId != 0 ? subcategoryId : c.SubcategoryId;
                    _dbContext.Entry(c).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return true;
                } else {
                    throw new Exception("Something went wrong - rollback db...");
                }
            } catch {
                await transaction.RollbackToSavepointAsync("BeforeUpdateContact");
                return false;
            }
        }
    }
}
