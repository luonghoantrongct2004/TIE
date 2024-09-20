namespace TIE_Decor.Entities;

public class Review
{
    public int ReviewId { get; set; }
    public Guid UserId { get; set; }
    public int? ProductId { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }

    // Quan hệ với ApplicationUser
    public virtual User User { get; set; }

    // Quan hệ với Product
    public virtual Product Product { get; set; }

}
