using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private AppDbContext _context;
        public ReportsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TrackPageVisit(string pageUrl = "/admin/reports/trackpagevisit")
        {
            if (string.IsNullOrWhiteSpace(pageUrl))
            {
                return BadRequest("Page URL cannot be null or empty.");
            }
            var pageVisit = new PageVisit { PageUrl = pageUrl };
            _context.PageVisits.Add(pageVisit);
            await _context.SaveChangesAsync();

            return Ok(); // Trả về phản hồi thành công
        }

        // Gọi khi người dùng rời khỏi trang
        [HttpPost]
        public async Task<IActionResult> TrackTimeSpent(string pageUrl, int timeSpent)
        {
            pageUrl = "/admin/reports/trackpagevisit";
            var timeSpentRecord = new PageTimeSpent { PageUrl = pageUrl, TimeSpent = timeSpent };
            _context.PageTimeSpents.Add(timeSpentRecord);
            await _context.SaveChangesAsync();

            return Ok(); // Trả về phản hồi thành công
        }

        // Gọi để lấy dữ liệu cho biểu đồ
        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            var visitsData = await _context.PageVisits
                .GroupBy(v => v.PageUrl)
                .Select(g => new
                {
                    PageUrl = g.Key,
                    VisitCount = g.Count()
                })
                .ToListAsync();

            var timeData = await _context.PageTimeSpents
                .GroupBy(t => t.PageUrl)
                .Select(g => new
                {
                    PageUrl = g.Key,
                    TotalTimeSpent = g.Sum(t => t.TimeSpent)
                })
                .ToListAsync();

            return Json(new { Visits = visitsData, TimeSpent = timeData });
        }
    }
}
