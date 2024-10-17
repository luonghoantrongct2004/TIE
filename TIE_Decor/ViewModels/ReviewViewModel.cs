using TIE_Decor.Entities;

namespace TIE_Decor.ViewModels
{
    public class ReviewViewModel
    {
        public string UserName { get; set; }  // To display the name of the user who posted the review
        public int Rating { get; set; }       // The rating given by the user (e.g., 1 to 5)
        public string Comment { get; set; }   // The actual review comment
        public string UserAvatar { get; set; }
    }
}
