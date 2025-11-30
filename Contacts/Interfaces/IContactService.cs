using Contacts.Models;

namespace Contacts.Interfaces
{
    public interface IContactService
    {
        /// <summary>
        /// Retrieves all contacts.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Contact>> GetAll();
        
        /// <summary>
        /// Retrieves a contact by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Contact> Get(Guid id);
        
        /// <summary>
        /// Updates a contact by its ID with the provided data.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        Task<bool> Update(Guid id, ContactDTO contact);
        
        /// <summary>
        /// Deletes a contact by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(Guid id);
    }
}
