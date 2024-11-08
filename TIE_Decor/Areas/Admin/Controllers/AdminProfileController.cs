using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TIE_Decor.Areas.Admin.Models;
using TIE_Decor.Entities;

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class AdminProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminProfileController(UserManager<User> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new AdminProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                ImageUrl = user.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm] AdminProfileViewModel model, IFormFile? file)
        {
            if (string.IsNullOrEmpty(model.FullName))
            {
                ModelState.AddModelError("FullName", "Full name is required.");
            }

            if (string.IsNullOrEmpty(model.Phone))
            {
                ModelState.AddModelError("Phone", "Phone number is required.");
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.Phone;

            // Handle password update if provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                if (model.Password.Length < 8)
                {
                    return Json(new { success = false, message = "Password must be at least 8 characters long." });
                }

                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
            }

            // Handle file upload (profile image)
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                user.ImageUrl = "/uploads/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Profile updated successfully." });
            }

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
           
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, errors =  "User not found." });
            }

            if (NewPassword != ConfirmPassword)
            {
                return Json(new { success = false, errors = "New password and confirmation password do not match." });
            }

            if (NewPassword.Length < 8)
            {
                return Json(new { success = false, errors = "Password must be at least 8 characters long." });
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return Json(new { success = false, errors = "Change password fail!!" });
            }

            return Json(new { success = true, message = "Password changed successfully." });
        }

    }
}
