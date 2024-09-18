using System.ComponentModel.DataAnnotations;

namespace TIE_Decor.Entities;

public class InteriorDesigner
{
    [Key]
    public int DesignerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ContactNumber { get; set; }
    public string Address { get; set; }
    public int YearsOfExperience { get; set; }
    public string Expertise { get; set; }
    public string Portfolio { get; set; }

    // Quan hệ 1-nhiều với Consultation (Tư vấn)
    public virtual ICollection<Consultation> Consultations { get; set; }

    // Quan hệ 1-nhiều với Review (Đánh giá)
    public virtual ICollection<Review> Reviews { get; set; }
}

