﻿namespace TIE_Decor.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string Category { get; set; }
    public string Brand { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }

    // Quan hệ 1-nhiều với Review (Đánh giá)
    public virtual ICollection<Review> Reviews { get; set; }
}

