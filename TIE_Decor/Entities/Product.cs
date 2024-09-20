namespace TIE_Decor.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }

    public List<string>? ImageUrl { get; set; }

    // Quan hệ 1-nhiều với Review (Đánh giá)
    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public virtual ICollection<Review>? Reviews { get; set; }
}

