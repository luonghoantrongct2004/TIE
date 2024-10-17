    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TIE_Decor.DbContext;
    using TIE_Decor.Entities;
using TIE_Decor.Service;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user1 = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user1 != null)
            {
                // Lưu trữ đối tượng user vào Session dưới dạng JSON
                HttpContext.Session.SetObject("CurrentUser", user1);
            }
            ViewData["user"] = user1;

            var designerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(designerIdClaim, out Guid designerIdGuid))
                {
                    return BadRequest("Invalid DesignerId.");
                }
          

            var consultations = await _context.Consultations
                    .Include(c => c.User)
                    .Where(c => c.DesignerID == designerIdGuid)
                    .ToListAsync();
            foreach (var consultation in consultations)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == consultation.UserId.ToString());

                if (user != null)
                {
                    // Gán thông tin User vào Consultation hoặc xử lý dữ liệu
                    consultation.User = user;
                }
            }

            foreach (var consultation in consultations)
            {
                Console.WriteLine($"Consultation ID: {consultation.ConsultationId}, User: {consultation.User?.FullName ?? "N/A"}");
            }

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
            var consultation = await _context.Consultations
                .FirstOrDefaultAsync(c => c.ConsultationId == id);

            if (consultation == null)
            {
                return NotFound("Buổi tư vấn không tồn tại.");
            }

            consultation.Status = "Confirmed";

            var designerSchedule = await _context.DesignerSchedules
                .FirstOrDefaultAsync(ds => ds.DesignerId == consultation.DesignerID
                                        && ds.ScheduledTime == consultation.ScheduledTime);

            if (designerSchedule != null)
            {
                designerSchedule.Status = "Confirmed";
            }
            else
            {
           
            }

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

            // Set the consultation status to "Declined"
            consultation.Status = "Declined";

            // Find the associated designer schedule
            var designerSchedule = await _context.DesignerSchedules
                .FirstOrDefaultAsync(ds => ds.DesignerId == consultation.DesignerID
                                            && ds.ScheduledTime == consultation.ScheduledTime);

            // If the schedule exists, update its status
            if (designerSchedule != null)
            {
                designerSchedule.Status = "Canceled by Designer";
            }

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
                return NotFound();
            }

            _context.DesignerSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult ConfirmSchedule(int consultationId)
        {
            var consultation = _context.Consultations
                                       .FirstOrDefault(c => c.ConsultationId == consultationId);

            if (consultation == null)
            {
                return NotFound();  
            }

            var designerSchedule = _context.DesignerSchedules
                                           .FirstOrDefault(ds => ds.DesignerId == consultation.DesignerID
                                                                 && ds.ScheduledTime == consultation.ScheduledTime);

            if (designerSchedule == null)
            {
                return NotFound(); 
            }

            if (designerSchedule.Status == "Pending")
            {
                designerSchedule.Status = "Confirmed";

                consultation.Status = "Booked";

                _context.SaveChanges();

                return Ok(new { message = "Consultation and Designer Schedule confirmed successfully." });
            }

            return BadRequest(new { message = "The schedule is not in a Pending state." });
        }
        [HttpPost]
        [Route("admin/consultationmanage/DeleteSchedule/{id:int}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var schedule = await _context.DesignerSchedules.FindAsync(id);
                if (schedule == null)
                {
                    return Json(new { success = false, message = "Schedule not found." });
                }

                var associatedConsultation = await _context.Consultations
                    .FirstOrDefaultAsync(c => c.DesignerID == schedule.DesignerId && c.ScheduledTime == schedule.ScheduledTime);

                if (associatedConsultation != null)
                {
                    return Json(new
                    {
                        success = false,
                        errorCode = "SCHEDULE_HAS_CONSULTATION",
                        message = "Cannot delete this schedule because it has an associated consultation."
                    });
                }

                _context.DesignerSchedules.Remove(schedule);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Schedule has been successfully deleted." });
            }
            catch (Exception ex)
            {
                // Log the exception here
                return Json(new { success = false, message = "An unexpected error occurred while deleting the schedule." });
            }
        }
    }
}
