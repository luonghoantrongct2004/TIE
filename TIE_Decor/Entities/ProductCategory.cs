using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TIE_Decor.Entities;

public class ProductCategory
{
    [Key]
    public int ProductCategoryId { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }

    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public string Description { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }
    public virtual Category Category { get; set; }
}