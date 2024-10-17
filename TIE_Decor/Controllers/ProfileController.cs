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

    // Kiểm tra ModelState
    if (!ModelState.IsValid)
    {
        return Json(new { success = false, errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage) });
    }

    var existingUser = await _userManager.FindByIdAsync(model.Id); 
    if (existingUser == null)
    {
        return NotFound();
    }

    // Cập nhật thông tin người dùng
    existingUser.FullName = model.FullName;
    existingUser.PhoneNumber = model.Phone;

    // Hash mật khẩu nếu được nhập
    if (!string.IsNullOrEmpty(model.Password))
    {
        var passwordHasher = new PasswordHasher<User>();
        existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, model.Password);
    }

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

    }
}
