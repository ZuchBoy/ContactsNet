using ContactsFront.Services;
using System;

namespace ContactsFront.Api;

public class ContactsApiClient
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;

    public ContactsApiClient(HttpClient http, AuthService authService)
    {
        _http = http;
        _auth = authService;
	}

    // Sets the bearer token for HTTP requests using the AuthService.
    public async Task<bool> SetBearerToken() {
        try {
            string token = await _auth.GetTokenSafeAsync();

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return true;
        } catch {
            return false;
        }
    }

    // GET /Contact
    // Retrieves a list of contacts from the backend API.
    public async Task<List<Contact>?> GetContactsAsync()
    {
        return await _http.GetFromJsonAsync<List<Contact>>("/Contact");
    }

    // GET /Contact/{id}
    // Retrieves a single contact by its ID from the backend API.
    public async Task<Contact?> GetContactAsync(Guid id)
    {
        await SetBearerToken();
        return await _http.GetFromJsonAsync<Contact>($"/Contact/{id}");
    }

    // PUT /Contact/{id}
    // Updates a contact with new information and subcategory name.
    public async Task<HttpResponseMessage> UpdateContactAsync(Guid id, Contact contact, string newSubcategoryName)
    {
		await SetBearerToken();

        Subcategory? oldSubcategory = new();

        var old = await _http.GetFromJsonAsync<Contact>($"/Contact/{id}");
        if(old.SubcategoryId.HasValue)
			oldSubcategory = await _http.GetFromJsonAsync<Subcategory>($"/Category/subcategory/{old.SubcategoryId}");

		var oldBirthDate = old?.BirthDate.HasValue == true ? old.BirthDate.Value.ToString("yyyy-MM-dd") : "";
		ContactDTO oldContact = new ContactDTO(
            old.Id,
            old.FirstName,
            old.Surname,
            old.Email,
            old.Phone,
            oldBirthDate,
            old.CategoryId,
            string.IsNullOrEmpty(oldSubcategory.Name)? null : oldSubcategory.Name.Trim().ToLower()
            );

        ContactDTO updatedContact = new ContactDTO(
            contact.Id,
            contact.FirstName ?? "",
            contact.Surname ?? "",
            contact.Email ?? "",
            contact.Phone,
            contact.BirthDate.HasValue ? contact.BirthDate.Value.ToString("yyyy-MM-dd") : "",
            contact.CategoryId,
            newSubcategoryName.Trim().ToLower()
		);

        if (oldContact.Equals(updatedContact))
            return new HttpResponseMessage(System.Net.HttpStatusCode.NotModified);

		ContactDTO formatedContactToSave = new ContactDTO(
			updatedContact.Id,
			updatedContact.FirstName,
			updatedContact.Surname,
			updatedContact.Email,
			updatedContact.Phone,
			updatedContact.DateOfBirth,
			updatedContact.CategoryId,
	        newSubcategoryName
        );

		return await _http.PutAsJsonAsync($"/Contact/{id}", formatedContactToSave);
	}

    // DELETE /Contact/{id}
    // Deletes a contact by its ID from the backend API.
    public async Task<HttpResponseMessage> DeleteContactAsync(Guid id)
    {
		await SetBearerToken();
		return await _http.DeleteAsync($"/Contact/{id}");
    }
}

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
	public bool IsCustom { get; set; }
	public bool HasManySub { get; set; }
	public string? RowVersion { get; set; }
    public List<Contact>? Contacts { get; set; }
    public List<Subcategory>? Subcategories { get; set; }
}

public class Subcategory
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    public string? RowVersion { get; set; }
    public Category? Category { get; set; }
    public List<Contact>? Contacts { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public string? Username { get; set; }
    public string? PwdHash { get; set; }
    public string? RowVersion { get; set; }
    public Contact? Contact { get; set; }
}

public class Contact
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateOnly? BirthDate { get; set; }
    public int CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public string? RowVersion { get; set; }

    public Category? Category { get; set; }
    public Subcategory? Subcategory { get; set; }
    public List<User>? Users { get; set; }
}

public class RegisterModel {
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int CategoryId { get; set; }
    public string SubcategoryName { get; set; }
	public string Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
}

public record ContactDTO(Guid Id, string FirstName, string Surname, string Email, string? Phone, string DateOfBirth, int CategoryId, string SubcategoryName);
