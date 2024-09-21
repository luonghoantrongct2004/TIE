using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.DbContext;

namespace TIE_Decor.Areas.Admin.Controllers;

[Route("api/performance-data")]
[ApiController]
public class PerformanceController : ControllerBase
{
    private readonly AppDbContext _context;

    public PerformanceController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetPerformanceData([FromQuery] string timeFrame)
    {
        DateTime startDate = DateTime.MinValue;

        switch (timeFrame)
        {
            case "1M":
                startDate = DateTime.UtcNow.AddMonths(-1);
                break;
            case "6M":
                startDate = DateTime.UtcNow.AddMonths(-6);
                break;
            case "1Y":
                startDate = DateTime.UtcNow.AddYears(-1);
                break;
            case "all":
            default:
                startDate = DateTime.MinValue; // Không giới hạn thời gian
                break;
        }

        var pageViews = _context.PageViewTrackings
            .Where(p => p.ViewedAt >= startDate)
            .GroupBy(p => p.ViewedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .OrderBy(p => p.Month)
            .Select(p => p.Count)
            .ToArray();

        var clicks = _context.ClickTrackings
            .Where(c => c.TimeStamp >= startDate)
            .GroupBy(c => c.TimeStamp.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .OrderBy(c => c.Month)
            .Select(c => c.Count)
            .ToArray();

        var data = new
        {
            pageViews = pageViews,
            clicks = clicks
        };

        return Ok(data);
    }
}

[Route("api/map-markers")]
[ApiController]
public class MapController : ControllerBase
{
    [HttpGet]
    public IActionResult GetMapMarkers()
    {
        var markers = new[]
        {
        new { name = "Canada", coords = new double[] { 56.1304, -106.3468 } },
        new { name = "Brazil", coords = new double[] { -14.2350, -51.9253 } },
        new { name = "Russia", coords = new double[] { 61, 105 } },
        new { name = "China", coords = new double[] { 35.8617, 104.1954 } },
        new { name = "United States", coords = new double[] { 37.0902, -95.7129 } }
    };

        return Ok(markers);
    }
}
