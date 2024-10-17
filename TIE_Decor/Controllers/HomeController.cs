using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;
using System.Diagnostics;
using System.Security.Claims;
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
            ViewData["Blog"] = _context.Blog.ToList();
            ViewData["Products"] = _context.Products.ToList();
            return View(_context.Products.Include(c => c.Category).Include(b => b.Brand).ToList());
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Architecture()
        {
            return View();
        }
        public IActionResult InteriorDesign()
        {
            return View();
        }
        public IActionResult UrbanDesign()
        {
            return View();
        }
        public IActionResult Planning()
        {
            return View();
        }
        public IActionResult Modelling()
        {
            return View();
        }
        public IActionResult DecorPlan()
        {
            return View();
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
        public IActionResult ViewOrder()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Auth/Login");
            }

            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var orders = _context.Orders
                .Include(c => c.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(c => c.UserId == userId)
                .ToList();


            return View(orders);
        }
        [HttpPost]
        public JsonResult IsHeart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Json(new { success = false, message = "You must be logged in to add a product to favorites!" });
            }

            var fa = _context.Favorites.ToList();
            foreach (var item in fa)
            {
                if (item.ProductId == id)
                {
                    return Json(new { success = false, message = "Product is already in your favorites!" });
                }
            }

            var product = _context.Products.FirstOrDefault(i => i.ProductId == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found!" });
            }

            var favorite = new Favorite()
            {
                ProductId = product.ProductId,
                UserId = userId,
            };

            _context.Favorites.Add(favorite);
            _context.SaveChanges();

            return Json(new { success = true, message = "Product added to favorites!" });
        }


        public IActionResult Favorite()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_context.Favorites.Include(u => u.User)
                .Include(p => p.Product)
                .Where(u => u.UserId == userId)); 
        }
        public IActionResult Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) { return Redirect("/Auth/Login"); }
            var favorite = _context.Favorites.FirstOrDefault(i => i.Id == id);

            _context.Favorites.Remove(favorite);
            _context.SaveChanges();
            return Redirect("/home/Favorite");
        }
    }
}
