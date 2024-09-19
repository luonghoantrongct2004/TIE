namespace TIE_Decor.Entities;

public class Cart
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Guid UserId { get; set; }

    public User? User { get; set; }
    public List<Product>? Products { get; set; }
}
