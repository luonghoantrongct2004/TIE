using Microsoft.AspNetCore.Mvc;
using TIE_Decor.DbContext;
using TIE_Decor.Models;

namespace TIE_Decor.Controllers;

[Route("api/page-views")]
[ApiController]
public class PageViewTrackingController : ControllerBase
{
    private readonly AppDbContext _context;

    public PageViewTrackingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> TrackPageView([FromBody] PageViewData pageViewData)
    {
        if (ModelState.IsValid)
        {
            var pageView = new PageViewTracking
            {
                PageUrl = pageViewData.PageUrl,
                ViewedAt = DateTime.UtcNow
            };

            _context.PageViewTrackings.Add(pageView);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        return BadRequest();
    }
}

public class PageViewData
{
    public string PageUrl { get; set; }
    public DateTime TimeStamp { get; set; }
}

