using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIE_Decor.Entities;
using TIE_Decor.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TIE_Decor.Controllers
{
    public class ConsultationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ConsultationController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> ScheduledAppointments()
        {
            var designers = await _userManager.GetUsersInRoleAsync("Designer");
            var designerList = designers.Select(d => new
            {
                Id = d.Id,
                FullName = d.FullName,
                ImageUrl = d.ImageUrl,
                YearsOfExperience = d.YearsOfExperience,
                Expertise = d.Expertise,
                Portfolio = d.Portfolio
            }).ToList();

            ViewBag.Designers = designerList;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetScheduledAppointments(Guid designerId)
        {
            try
            {
                var scheduledAppointments = await _context.DesignerSchedules
                    .Where(s => s.DesignerId == designerId)
                    .ToListAsync();

                return Json(scheduledAppointments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching scheduled appointments: {ex.Message}");
                return BadRequest(new { success = false, message = "Error fetching scheduled appointments." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ScheduleConsultation(Guid scheduleId)
        {
            var schedule = await _context.DesignerSchedules.FindAsync(scheduleId);
            if (schedule == null || schedule.Status != "Available")
            {
                return Json(new { success = false, message = "This time slot is not available" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (!Guid.TryParse(currentUser.Id, out Guid userId))
            {
                return Json(new { success = false, message = "User ID is not valid." });
            }

            try
            {
                var consultation = new Consultation
                {
                    UserId = userId,
                    DesignerID = schedule.DesignerId,
                    ScheduledTime = schedule.ScheduledTime,
                    Status = "Pending",
                    Notes = $"Consultation scheduled at {schedule.ScheduledTime}. Awaiting confirmation."
                };

                _context.Consultations.Add(consultation);
                schedule.Status = "Pending";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Consultation scheduled successfully. Awaiting designer confirmation." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to schedule consultation", error = ex.Message });
            }
        }

        [HttpGet("ConfirmConsultation/{appointmentId}")]
        public async Task<IActionResult> ConfirmConsultation(int appointmentId)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var consultationS = _context.DesignerSchedules.FirstOrDefault(f => f.ScheduleId == appointmentId);

            if (consultationS == null)
            {
                return Json(new { success = false, message = "Consultation not found." });
            }

            var consultation = new Consultation()
            {
                UserId = Guid.Parse(user),
                DesignerID = consultationS.DesignerId,
                Status = "Pending",
                Notes = consultationS.Notes,
            };

            _context.Consultations.Add(consultation);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Consultation confirmed successfully." });
        }

    }
}