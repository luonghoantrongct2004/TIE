namespace TIE_Decor.Entities;

public class Review
{
    public int ReviewId { get; set; }
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    // Quan hệ với ApplicationUser
    public User? User { get; set; }

    // Quan hệ với Product
    public Product? Product { get; set; }

}