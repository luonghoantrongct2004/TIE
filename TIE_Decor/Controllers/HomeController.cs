using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;
using TIE_Decor.Models;

namespace TIE_Decor.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        

		public HomeController(AppDbContext context)
		{
			_context = context;
		}
        public IActionResult Index()
        {
            return View(_context.Products.Include(c => c.Category).Include(b => b.Brand).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Blog(int page = 1, int pageSize = 5)
        {
            // Tổng số blog
            var totalBlogs = _context.Blog.Count();

            // Lấy danh sách blog cho trang hiện tại
            var blogs = _context.Blog
                .OrderByDescending(b => b.UpdatedDate)  // Sắp xếp blog mới nhất lên đầu
                .Skip((page - 1) * pageSize)  // Bỏ qua các blog của các trang trước
                .Take(pageSize)  // Lấy số lượng blog cần cho trang hiện tại
                .ToList();

            // Tạo đối tượng chứa thông tin cần thiết cho view
            var model = new BlogViewModel
            {
                Blogs = blogs,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalBlogs / pageSize)
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            return View(_context.Blog.FirstOrDefault(d => d.Id == id));
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
