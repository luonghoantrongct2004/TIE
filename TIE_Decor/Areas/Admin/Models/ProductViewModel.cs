using TIE_Decor.Entities;

namespace TIE_Decor.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }  // Danh sách sản phẩm
        public int CurrentPage { get; set; }  // Trang hiện tại
        public int TotalPages { get; set; }  // Tổng số trang
    }

}
