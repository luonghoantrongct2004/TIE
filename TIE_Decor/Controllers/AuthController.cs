using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TIE_Decor.Entities;
using TIE_Decor.Models;

namespace TIE_Decor.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Hiển thị trang đăng nhập và đăng ký
        public IActionResult Auth()
        {
            var viewModel = new AuthViewModel
            {
                LoginModel = new LoginViewModel(),
                RegisterModel = new RegisterViewModel()
            };

            return View(viewModel);
        }

        // Hiển thị trang đăng nhập
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Trả về lỗi nếu model không hợp lệ
                return Json(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Đăng nhập thành công
                return Json(new { success = true, message = "Login successful" });
            }

            // Nếu đăng nhập thất bại
            return Json(new { success = false, message = "Invalid login attempt" });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data"});
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName
            };
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Json(new { success = true, message = "Registration successful" });
                }
                // Nếu đăng ký thất bại
                return Json(new { success = false, message = "Registration failed", errors = result.Errors.Select(e => e.Description) });
            }
            catch(Exception ex)
            {

                return Json(new { success = false, message = "Registration failed", errors = ex.Message });
            }
        }


    }
}
