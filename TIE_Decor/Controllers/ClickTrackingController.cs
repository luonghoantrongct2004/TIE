using Microsoft.AspNetCore.Mvc;
using TIE_Decor.DbContext;
using TIE_Decor.Models;

namespace TIE_Decor.Controllers
{
    [Route("api/clicks")]
    [ApiController]
    public class ClickTrackingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClickTrackingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> TrackClick([FromBody] ClickData clickData)
        {
            if (ModelState.IsValid)
            {
                // Lưu dữ liệu nhấp chuột vào cơ sở dữ liệu
                var click = new ClickTracking
                {
                    ElementId = clickData.ElementId,
                    TimeStamp = DateTime.UtcNow
                };

                _context.ClickTrackings.Add(click);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }

            return BadRequest();
        }
    }

    public class ClickData
    {
        public string ElementId { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
