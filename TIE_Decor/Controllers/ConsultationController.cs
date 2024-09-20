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

                var designers = await _context.InteriorDesigners
                    .Select(d => new { Id = d.DesignerId, FullName = d.FirstName + " " + d.LastName })
                    .ToListAsync();

                Console.WriteLine($"Number of designers retrieved: {designers.Count}");

                if (designers.Count == 0)
                {
                    Console.WriteLine("Warning: No designers found in the database.");
                }

                ViewBag.AvailableSlots = availableSlots;
                ViewBag.Designers = designers;

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
        public async Task<IActionResult> ScheduleConsultation(DateTime selectedTime, int designerId)
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

            var designer = await _context.InteriorDesigners.FindAsync(designerId);
            if (designer == null)
            {
                return Json(new { success = false, message = "Invalid designer selected" });
            }

            var isTimeTaken = await _context.Consultations
                .AnyAsync(c => c.ScheduledTime == selectedTime && c.DesignerId == designerId);
            if (isTimeTaken)
            {
                return Json(new { success = false, message = "This time slot is already taken for the selected designer" });
            }

            try
            {
                var consultation = new Consultation
                {
                    UserId = currentUser.Id,  // Đã thay đổi từ int.Parse(currentUser.Id) sang currentUser.Id
                    DesignerId = designerId,
                    ScheduledTime = selectedTime,
                    Status = "Scheduled"
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
                .Where(c => c.UserId == user.Id)  // Đã thay đổi từ int.Parse(user.Id) sang user.Id
                .Include(c => c.Designer)
                .ToListAsync();

            return View(consultations);
        }
    }
}