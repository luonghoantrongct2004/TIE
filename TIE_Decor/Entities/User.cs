﻿using Microsoft.AspNetCore.Identity;

namespace TIE_Decor.Entities;

public class User : IdentityUser
{
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ShippingAddress { get; set; }
    public string? ContactPhone { get; set; }
    // Quan hệ 1-nhiều với Consultation (Tư vấn)
    public virtual ICollection<Consultation>? Consultations { get; set; }

    // Quan hệ 1-nhiều với Review (Đánh giá)
    public virtual ICollection<Review>? Reviews { get; set; }

    // Quan hệ 1-nhiều với Design (Thiết kế)
    public virtual ICollection<Design>? Designs { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Expertise { get; set; }

    public string? Portfolio { get; set; }


}
