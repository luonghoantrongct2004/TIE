using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;
using TIE_Decor.Service; // Đảm bảo namespace đúng với dự án của bạn

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                // Lưu trữ đối tượng user vào Session dưới dạng JSON
                HttpContext.Session.SetObject("CurrentUser", user);
            }
            ViewBag.Orders = _context.Orders.Count();
            ViewBag.Products = _context.Products.Count();
            ViewBag.Category = _context.Categories.Count();
            ViewBag.Consulation = _context.Consultations.Count();
            ViewData["user"] = user;
            return View();
        }
    }
}
