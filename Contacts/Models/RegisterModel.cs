namespace Contacts.Models
{
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
}
