using Contacts.Interfaces;
using Contacts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        // Constructor that injects the contact service dependency.
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [AllowAnonymous]
        // Retrieves all contacts.
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _contactService.GetAll());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        // Retrieves a contact by its ID.
        public async Task<IActionResult> Get(Guid id)
        {
            var contact = await _contactService.Get(id);
            if (contact == null) {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPut("{id}")]
        [Authorize]
        // Updates a contact by its ID with the provided data.
        public async Task<IActionResult> Update(Guid id, [FromBody] ContactDTO contact)
        {
			return await _contactService.Update(id, contact) ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize]
        // Deletes a contact by its ID.
        public async Task<IActionResult> Delete(Guid id)
        {
            return await _contactService.Delete(id) ? NoContent() : NotFound();
        }

        [HttpGet("debug/token")]
        [AllowAnonymous]
        // Returns the current Authorization header for debugging purposes.
        public IActionResult DebugToken()
        {
            return Ok(new
            {
                AuthHeader = Request.Headers.Authorization.ToString()
            });
        }

    }
}
