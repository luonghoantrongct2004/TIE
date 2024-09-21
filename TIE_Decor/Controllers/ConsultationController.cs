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
        public async Task<IActionResult> AvailableSlots()
        {
            try
            {
                var availableSlots = GetAvailableSlots();

                var designers = await _userManager.GetUsersInRoleAsync("Designer");
                var designerList = designers.Select(d => new { Id = d.Id, FullName = d.FullName }).ToList();

                Console.WriteLine($"Number of designers retrieved: {designerList.Count}");

                if (designerList.Count == 0)
                {
                    Console.WriteLine("Warning: No designers found in the database.");
                }

                ViewBag.AvailableSlots = availableSlots;
                ViewBag.Designers = designerList;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AvailableSlots: {ex.Message}");
                return View("Error");
            }
        }

        private List<DateTime> GetAvailableSlots()
        {
            DateTime startTime = DateTime.Now.Date.AddHours(9);
            DateTime endTime = DateTime.Now.Date.AddHours(17);

            var consultations = _context.Consultations
                                        .Where(c => c.ScheduledTime.Date == DateTime.Now.Date)
                                        .Select(c => c.ScheduledTime)
                                        .ToList();

            List<DateTime> availableSlots = new List<DateTime>();

            for (var time = startTime; time < endTime; time = time.AddHours(1))
            {
                if (!consultations.Contains(time))
                {
                    availableSlots.Add(time);
                }
            }

            return availableSlots;
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleConsultation(DateTime selectedTime, string designerId)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
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

            var isTimeTaken = await _context.Consultations
                .AnyAsync(c => c.ScheduledTime == selectedTime && c.User.Id == designerId);
            if (isTimeTaken)
            {
                return Json(new { success = false, message = "This time slot is already taken for the selected designer" });
            }

            try
            {
                var consultation = new Consultation
                {
                    User = currentUser,
                    ScheduledTime = selectedTime,
                    Status = "Đã đặt lịch",
                    Notes = $"Consultation scheduled with designer {designer.FullName}"
                };

                _context.Consultations.Add(consultation);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Consultation scheduled successfully with the selected designer" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to schedule consultation", errors = ex.Message });
            }
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
                .Where(c => c.User.Id == user.Id)
                .ToListAsync();

            return View(consultations);
        }
    }
}