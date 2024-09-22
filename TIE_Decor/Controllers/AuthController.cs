﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
                return Json(new { success = false, message = "Invalid data" });
            }

            // Tìm người dùng dựa trên email
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Json(new { success = false, message = "Email not found" });
            }

            // Kiểm tra đăng nhập bằng email và mật khẩu
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.FullName));

                return Json(new { success = true, message = "Login successful" });
            }
            else
            {
                return Json(new { success = false, message = "Invalid login attempt" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                return Json(new { success = false, message = errors });
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.FullName));

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Json(new { success = true, message = "Registration successful" });
                }
                // Nếu đăng ký thất bại
                return Json(new { success = false, message = "Registration failed", errors = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Registration failed", errors = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDesigner(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.FullName));

                    await _userManager.AddToRoleAsync(user, "Designer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Json(new { success = true, message = "Registration successful" });
                }
                // Nếu đăng ký thất bại
                return Json(new { success = false, message = "Registration failed", errors = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Registration failed", errors = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "/");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Designer"))
            {
                return Forbid(); // or redirect to an appropriate page
            }

            var model = new User
            {
                FullName = user.FullName,
                ImageUrl = user.ImageUrl,
                YearsOfExperience = user.YearsOfExperience,
                Expertise = user.Expertise,
                Portfolio = user.Portfolio,
            };

            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}