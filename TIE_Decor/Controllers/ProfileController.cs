using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using TIE_Decor.Areas.Admin.Models;
using TIE_Decor.Entities;
using TIE_Decor.Models;

namespace TIE_Decor.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProfileController(UserManager<User> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                Phone = user.PhoneNumber
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model, IFormFile file)
        {
            // Kiểm tra xem các trường bắt buộc có rỗng hay không
            if (string.IsNullOrEmpty(model.FullName))
            {
                ModelState.AddModelError("FullName", "Full name is required.");
            }

            if (string.IsNullOrEmpty(model.Phone))
            {
                ModelState.AddModelError("Phone", "Phone number is required.");
            }

            if (!string.IsNullOrEmpty(model.Password) && model.Password.Length < 8)
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long.");
            }


            var existingUser = await _userManager.FindByIdAsync(model.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            existingUser.FullName = model.FullName;
            existingUser.PhoneNumber = model.Phone;



            // Xử lý upload ảnh
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                existingUser.ImageUrl = "/uploads/" + uniqueFileName;
            }

            try
            {
                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Change information successfully!" });
                }
                else
                {
                    return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { "Error occurred when changing information: " + ex.Message } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            // Create a list to store errors
            var errors = new List<string>();

            // Step 1: Check if any fields are empty
            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                TempData["Err"] = "All fields are required.";
            }

            // Step 2: Check if NewPassword and ConfirmPassword match
            if (NewPassword != ConfirmPassword)
            {
                TempData["Err"] = "New password and confirm password do not match.";
            }

            // Step 3: Retrieve the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Err"] = "User not authenticated.";
            }

            // Step 4: Verify if the provided CurrentPassword is correct
            if (errors.Count == 0) // Only check password if there are no other errors
            {
                var passwordIsValid = await _userManager.CheckPasswordAsync(user, CurrentPassword);
                if (!passwordIsValid)
                {
                    TempData["Err"] = "Current password is incorrect.";
                }
            }

            // Step 5: Update the password if no errors
            if (errors.Count == 0)
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
                if (passwordChangeResult.Succeeded)
                {
                    TempData["Success"] = "Password changed successfully!";
                }
            }

            // If successful, redirect to a confirmation page or the profile view
            return RedirectToAction("UpdateProfile");

        }

    }

}
