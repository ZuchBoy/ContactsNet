using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Models;

[Table("Contact")]
[Index("CategoryId", Name = "IX_Category")]
[Index("Email", Name = "IX_Email")]
[Index("SubcategoryId", Name = "IX_Subcategory")]
[Index("Email", Name = "UQ__Contact__A9D1053434999310", IsUnique = true)]
public partial class Contact
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string Surname { get; set; } = null!;

    [StringLength(300)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? Phone { get; set; }

    public DateOnly? BirthDate { get; set; }

    public int CategoryId { get; set; }

    public int? SubcategoryId { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Contacts")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("SubcategoryId")]
    [InverseProperty("Contacts")]
    public virtual Subcategory? Subcategory { get; set; }

    [InverseProperty("Contact")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
