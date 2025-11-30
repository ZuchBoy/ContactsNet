using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Models;

[Table("Category")]
[Index("Name", Name = "UQ__Category__737584F66BB4572D", IsUnique = true)]
public partial class Category
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

	public bool IsCustom { get; set; }

	public bool HasManySub { get; set; }

	public byte[] RowVersion { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    [InverseProperty("Category")]
    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
