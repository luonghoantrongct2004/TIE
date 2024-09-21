using TIE_Decor.DbContext;
using TIE_Decor.Models;

namespace TIE_Decor.Service
{
    public class TrackingService : ITrackingService
    {
        private readonly AppDbContext _context;

        public TrackingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task TrackPageView(string path)
        {
            // Ghi thông tin vào cơ sở dữ liệu
            var pageView = new PageViewTracking
            {
                PageUrl = path,
                ViewedAt = DateTime.UtcNow
            };

            _context.PageViewTrackings.Add(pageView);
            await _context.SaveChangesAsync();
        }
    }

}
