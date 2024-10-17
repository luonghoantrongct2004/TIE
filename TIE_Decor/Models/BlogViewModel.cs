using TIE_Decor.Entities;

namespace TIE_Decor.Models
{
    public class BlogViewModel
    {
        public IEnumerable<Blog> Blogs { get; set; }  // Danh sách blog
        public int CurrentPage { get; set; }  // Trang hiện tại
        public int TotalPages { get; set; }  // Tổng số trang
    }

}
