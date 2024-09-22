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
using Azure.Core;

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
                Id = d.Id.ToString(),
                FullName = d.FullName,
                ImageUrl = d.ImageUrl,
                YearsOfExperience = d.YearsOfExperience,
                Expertise = d.Expertise,
                Portfolio = d.Portfolio
            }).ToList();

            // Lấy danh sách ID của các designer
            var designerIds = designerList.Select(d => d.Id).ToList();
            ViewBag.Designers = designerList;

            // Lấy đánh giá cho từng designer
            var reviews = await _context.Reviews
                .Where(r => designerIds.Contains(r.UserId1.ToString()))
                .Select(r => new
                {
                    r.ReviewId,
                    r.Rating,
                    r.Comment,
                    UserId = r.UserId1.ToString(), // Lưu ID của designer
                    Username = _context.Users.FirstOrDefault(u => u.Id == r.UserId.ToString()).FullName
                }).ToListAsync();

            ViewBag.Reviews = reviews; // Kiểm tra kỹ rằng dữ liệu reviews có thực sự đúng

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
                var reviews = await _context.Reviews
                    .Where(r => designerId == r.UserId1)
                    .Select(r => new
                    {
                        r.ReviewId,
                        r.Rating,
                        r.Comment,
                        UserId = r.UserId1.ToString(), // Lưu ID của designer
                        Username = _context.Users.FirstOrDefault(u => u.Id == r.UserId.ToString()).FullName // Lấy tên người dùng
                    }).ToListAsync();

                // Trả về cả danh sách scheduledAppointments và reviews
                return Json(new { success = true, scheduledAppointments, reviews });

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
        [HttpGet]
        [Route("Consultation/ConfirmConsultation/{scheduleId:int}")]
        public async Task<IActionResult> ConfirmConsultation(int scheduleId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                var schedule = await _context.DesignerSchedules.FindAsync(scheduleId);
                if (schedule == null)
                {
                    return Json(new { success = false, message = "Schedule not found." });
                }

                if (schedule.Status != "Available")
                {
                    return Json(new { success = false, message = "This time slot is no longer available." });
                }

                // Cập nhật trạng thái thành "Pending" thay vì "Booked"
                schedule.Status = "Pending";
                await _context.SaveChangesAsync();

                // Tạo cuộc hẹn
                var consultation = new Consultation
                {
                    UserId = Guid.Parse(userId),
                    DesignerID = schedule.DesignerId,
                    ScheduledTime = schedule.ScheduledTime,
                    Status = "Pending", // Giữ trạng thái là Pending
                    Notes = schedule.Notes
                };

                _context.Consultations.Add(consultation);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Consultation is now pending and waiting for admin confirmation." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConfirmConsultation: {ex}");
                return Json(new { success = false, message = "An error occurred while confirming the consultation.", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("Consultation/History")]
        public async Task<IActionResult> AppointmentHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Truy xuất danh sách cuộc hẹn cho người dùng hiện tại
            var consultations = await _context.Consultations
                .Where(c => c.UserId == Guid.Parse(userId))
                .ToListAsync();

            // Tạo danh sách lịch sử cuộc hẹn với các thông tin cần thiết
            var appointmentHistory = consultations.Select(c => new
            {
                c.ConsultationId, // Thêm ConsultationId để dùng cho hủy cuộc hẹn
                c.ScheduledTime,
                c.Status,
                c.Notes,
                DesignerName = _context.Users.FirstOrDefault(u => u.Id == c.DesignerID.ToString())?.FullName // Lấy tên designer
            }).ToList();

            return View(appointmentHistory); // Trả về danh sách cuộc hẹn cho view
        }
        [HttpPost]
        [Route("Consultation/Cancel/{id}")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] string notes)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return Json(new { success = false, message = "Appointment not found." });
            }

            if (consultation.UserId != Guid.Parse(userId))
            {
                return Json(new { success = false, message = "You are not authorized to cancel this appointment." });
            }

            if (consultation.Status == "Cancelled")
            {
                return Json(new { success = false, message = "This appointment is already cancelled." });
            }

            consultation.Status = "Cancelled";
            consultation.Notes = $"Reason: {notes} (Cancelled on {DateTime.Now.ToString("f")})"; 

            var schedule = await _context.DesignerSchedules
                .FirstOrDefaultAsync(s => s.DesignerId == consultation.DesignerID && s.ScheduledTime == consultation.ScheduledTime);

            if (schedule != null)
            {
                schedule.Status = "Available";
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Appointment cancelled successfully." });
        }
        [HttpGet]
        [Route("Consultation/GetDesignerReviews/{designerId}")]
        public async Task<IActionResult> GetDesignerReviews(Guid designerId)
        {
            try
            {
                // Truy vấn các đánh giá từ bảng Review dựa trên UserId1 (designerId)
                var reviews = await _context.Reviews
                    .Where(r => r.UserId1 == designerId)
                    .Select(r => new
                    {
                        r.ReviewId,
                        r.Rating,
                        r.Comment,
                        Username = _context.Users.FirstOrDefault(u => u.Id == r.UserId.ToString()).FullName 
                    })
                    .ToListAsync();

                if (!reviews.Any())
                {
                    return Json(new { success = true, message = "No reviews found for this designer." });
                }

                return Json(new { success = true, reviews });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reviews: {ex.Message}");
                return BadRequest(new { success = false, message = "Error fetching reviews.", error = ex.Message });
            }
        }

    }
}