using TIE_Decor.Entities;

namespace TIE_Decor.Models
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public Guid UserId { get; set; }
        public Guid UserId1 { get; set; } 
        public int? ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
