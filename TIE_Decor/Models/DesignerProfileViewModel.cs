namespace TIE_Decor.Models
{
    public class DesignerProfileViewModel : UserProfileViewModel
    {
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public int? YearsOfExperience { get; set; }
        public string Expertise { get; set; }
        public string Portfolio { get; set; }
        public DateTime? AvailableDate { get; set; } // Thêm thuộc tính này

    }
}
