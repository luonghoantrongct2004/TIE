﻿namespace TIE_Decor.Entities;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Product>? Products { get; set; }
}
