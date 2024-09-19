﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Accounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }
        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return Redirect("/admin/users");
        }
    }
}
