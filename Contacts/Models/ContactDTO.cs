namespace Contacts.Models {
	public record ContactDTO(Guid Id, string FirstName, string Surname, string Email, string? Phone, string DateOfBirth, int CategoryId, string SubcategoryName);
}
