﻿using Microsoft.AspNetCore.Identity;

namespace TIE_Decor.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; }
    // Quan hệ 1-nhiều với Consultation (Tư vấn)
    public string? ImageUrl { get; set; }

    public virtual ICollection<Consultation> Consultations { get; set; }

    // Quan hệ 1-nhiều với Review (Đánh giá)
    public virtual ICollection<Review> Reviews { get; set; }

    // Quan hệ 1-nhiều với Design (Thiết kế)
    public virtual ICollection<Design> Designs { get; set; }
}
