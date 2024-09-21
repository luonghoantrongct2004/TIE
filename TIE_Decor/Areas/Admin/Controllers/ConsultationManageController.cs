    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TIE_Decor.DbContext;
    using TIE_Decor.Entities;

    namespace TIE_Decor.Areas.Admin.Controllers
    {
        [Area("Admin")]
        public class ConsultationManageController : Controller
        {
            private readonly AppDbContext _context;

            public ConsultationManageController(AppDbContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Index()
            {
                var designerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(designerIdClaim, out Guid designerIdGuid))
                {
                    return BadRequest("Invalid DesignerId.");
                }

                var consultations = await _context.Consultations
                    .Include(c => c.User)
                    .Where(c => c.DesignerID == designerIdGuid)
                    .ToListAsync();

                var schedules = await _context.DesignerSchedules
                    .Where(s => s.DesignerId == designerIdGuid)
                    .ToListAsync();

                var viewModel = (Consultations: consultations.AsEnumerable(), Schedules: schedules.AsEnumerable());
                return View(viewModel);
            }
            [HttpPost]
            [Route("admin/consultationmanage/Confirm/{id:int}")]
            public async Task<IActionResult> Confirm(int id)
            {
           
                var consultation = await _context.Consultations.FindAsync(id);
                if (consultation == null)
                {
                    return NotFound("Buổi tư vấn không tồn tại.");
                }

         
                consultation.Status = "Confirmed";
                await _context.SaveChangesAsync();

         
                return Redirect("/admin/consultationmanage");
            }
        
            [HttpPost]
            [Route("admin/consultationmanage/Decline/{id:int}")]
            public async Task<IActionResult> Decline(int id)
            {
                var consultation = await _context.Consultations.FindAsync(id);
                if (consultation == null)
                {
                    return NotFound("Buổi tư vấn không tồn tại.");
                }

         
                consultation.Status = "Declined";
                await _context.SaveChangesAsync();
                return Redirect("/admin/consultationmanage");
            }

       

            public async Task<IActionResult> Delete(int id)
            {
                var consultation = await _context.Consultations.FindAsync(id);
                if (consultation == null)
                {
                    return NotFound("Buổi tư vấn không tồn tại.");
                }

                _context.Consultations.Remove(consultation);
                await _context.SaveChangesAsync();

                return Redirect("/admin/consultationmanage");
            }

            [HttpGet]
            public IActionResult CreateSchedule()
            {

                return View(); // Khởi tạo model
            }
            [HttpPost]
            public async Task<IActionResult> CreateSchedule(DesignerSchedules schedule)
            {
                if (ModelState.IsValid)
                {
                    var designerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!Guid.TryParse(designerIdClaim, out Guid designerIdGuid))
                    {
                        return BadRequest("Invalid DesignerId.");
                    }

                    schedule.DesignerId = designerIdGuid;
                    schedule.Status = "Available";

                    _context.DesignerSchedules.Add(schedule);
                    await _context.SaveChangesAsync();
                    return Redirect("/admin/consultationmanage");
                }
                return View(schedule);
            }
            public async Task<IActionResult> ViewSchedule()
            {
                var designerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(designerIdClaim, out Guid designerIdGuid))
                {
                    return BadRequest("Invalid DesignerId.");
                }

                var schedules = await _context.DesignerSchedules
                    .Where(s => s.DesignerId == designerIdGuid)
                    .ToListAsync();
                return View(schedules);
            }
            [HttpPost]
            public async Task<IActionResult> CancelSchedule(int id)
            {
                var schedule = await _context.DesignerSchedules.FindAsync(id);
                if (schedule == null)
                {
                    return NotFound("Lịch không tồn tại.");
                }

                var designerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(designerIdClaim, out Guid designerIdGuid) || schedule.DesignerId != designerIdGuid)
                {
                    return Unauthorized("Bạn không có quyền hủy lịch này.");
                }

                _context.DesignerSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewSchedule));
            }
        }
    }
