using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIE_Decor.Entities;
using TIE_Decor.DbContext;
using Microsoft.EntityFrameworkCore;

namespace TIE_Decor.Controllers
{
    public class ConsultationClientController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ConsultationClientController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AvailableSlots()
        {
            try
            {
                var availableSlots = GetAvailableSlotsForWeek();
                var consultations = await _context.Consultations
                    .Where(c => c.ScheduledTime >= DateTime.Now.Date && c.ScheduledTime < DateTime.Now.Date.AddDays(7))
                    .ToListAsync();

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

                ViewBag.AvailableSlots = availableSlots;
                ViewBag.Consultations = consultations;
                ViewBag.Designers = designerList;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AvailableSlots: {ex.Message}");
                return View("Error");
            }
        }
        private Dictionary<DateTime, List<DateTime>> GetAvailableSlotsForWeek()
        {
            DateTime startOfWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek); // Start of the week
            DateTime endOfWeek = startOfWeek.AddDays(7); // End of the week

            var consultations = _context.Consultations
                .Where(c => c.ScheduledTime >= startOfWeek && c.ScheduledTime < endOfWeek)
                .ToList();

            var availableSlots = new Dictionary<DateTime, List<DateTime>>();

            for (var date = startOfWeek; date < endOfWeek; date = date.AddDays(1))
            {
                var dailySlots = new List<DateTime>();
                for (var time = date.AddHours(9); time < date.AddHours(17); time = time.AddHours(1))
                {
                    // Exclude past slots for the current day
                    if (date.Date == DateTime.Now.Date && time < DateTime.Now)
                        continue;

                    if (!consultations.Any(c => c.ScheduledTime == time))
                    {
                        dailySlots.Add(time);
                    }
                }
                availableSlots[date] = dailySlots;
            }

            return availableSlots;
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleConsultation(DateTime selectedTime, string designerId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Key = x.Key, Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToArray() })
                    .ToArray();

                return Json(new { success = false, message = "Invalid data", errors = errors });
            }

            // Check if designerId is provided
            if (string.IsNullOrEmpty(designerId))
            {
                return Json(new { success = false, message = "Designer ID is required", errors = new[] { new { key = "designerId", messages = new[] { "The designerId field is required." } } } });
            }
            if (!Guid.TryParse(designerId, out Guid designerGuid))
            {
                return Json(new { success = false, message = "Invalid Designer ID format" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var designer = await _userManager.FindByIdAsync(designerId);
            if (designer == null || !await _userManager.IsInRoleAsync(designer, "Designer"))
            {
                return Json(new { success = false, message = "Invalid designer selected" });
            }

            // Check if the selected time is valid
            if (selectedTime < DateTime.Now)
            {
                return Json(new { success = false, message = "Cannot schedule consultations in the past" });
            }

            if (selectedTime.Hour < 9 || selectedTime.Hour >= 17)
            {
                return Json(new { success = false, message = "Consultations are only available between 9 AM and 5 PM" });
            }

            var isTimeTaken = await _context.Consultations
                .AnyAsync(c => c.ScheduledTime == selectedTime && (c.DesignerID == designerGuid || c.UserId == Guid.Parse(currentUser.Id)));
            if (isTimeTaken)
            {
                return Json(new { success = false, message = "This time slot is already taken" });
            }

            try
            {
                var consultation = new Consultation
                {
                    UserId = Guid.Parse(currentUser.Id),
                    DesignerID = designerGuid,
                    ScheduledTime = selectedTime,
                    Status = "Pending",  // Set status as "Pending"
                    Notes = $"Consultation scheduled with designer {designer.FullName}"
                };

                _context.Consultations.Add(consultation);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Consultation scheduled successfully with the selected designer" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to schedule consultation", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelConsultation(int consultationId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var consultation = await _context.Consultations
                .FirstOrDefaultAsync(c => c.ConsultationId == consultationId && c.UserId == Guid.Parse(currentUser.Id));

            if (consultation == null)
            {
                return Json(new { success = false, message = "Consultation not found or you don't have permission to cancel it" });
            }

            if (consultation.ScheduledTime < DateTime.Now)
            {
                return Json(new { success = false, message = "Cannot cancel past consultations" });
            }

            consultation.Status = "Đã hủy";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Consultation cancelled successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> MyConsultations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var consultations = await _context.Consultations
                .Where(c => c.UserId == Guid.Parse(user.Id))
                .ToListAsync();

            return View(consultations);
        }
    }
}
