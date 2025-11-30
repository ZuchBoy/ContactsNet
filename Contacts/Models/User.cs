using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Models;

[Table("User")]
[Index("ContactId", Name = "IX_ContactId")]
[Index("Username", Name = "UQ__User__536C85E40749FAF7", IsUnique = true)]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    public Guid ContactId { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    public string PwdHash { get; set; } = null!;

    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("ContactId")]
    [InverseProperty("Users")]
    public virtual Contact Contact { get; set; } = null!;
}
