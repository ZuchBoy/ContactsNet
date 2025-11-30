using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Models;

[Table("Subcategory")]
[Index("Name", Name = "UQ__Subcateg__737584F65AC02947", IsUnique = true)]
public partial class Subcategory
{
    [Key]
    public int Id { get; set; }

    public int CategoryId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Subcategories")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Subcategory")]
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
