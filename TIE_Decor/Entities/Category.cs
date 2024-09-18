namespace TIE_Decor.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    // Quan hệ 1-nhiều với Product (Sản phẩm)
    public virtual ICollection<Product> Products { get; set; }
}
