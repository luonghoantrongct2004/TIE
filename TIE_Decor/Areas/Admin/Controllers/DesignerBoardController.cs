using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DesignerBoardController : Controller
    {
        private readonly AppDbContext _context;

        public DesignerBoardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DesignerBoard
        public async Task<IActionResult> Index()
        {
            // Lấy DesignerId từ token đăng nhập
            var designerIdClaim = HttpContext.User.FindFirst("DesignerId"); // "DesignerId" là tên của claim trong token
            if (designerIdClaim == null)
            {
                return Unauthorized("Không tìm thấy DesignerId trong token.");
            }

            string designerId = designerIdClaim.Value;

            // Convert designerId to an int if necessary
            if (!int.TryParse(designerId, out int designerIdInt))
            {
                return BadRequest("Invalid DesignerId.");
            }

            // Lấy danh sách các buổi tư vấn có DesignerId bằng giá trị từ token
            var consultations = await _context.Consultations
                .Include(c => c.User)
                .Include(c => c.Designer)
                .Where(c => c.DesignerId == designerIdInt) // Compare int to int
                .ToListAsync();

            return View(consultations);
        }

        // POST: Admin/DesignerBoard/Decline/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái buổi tư vấn thành "Hủy"
            consultation.Status = "Hủy";
            _context.Update(consultation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/DesignerBoard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation != null)
            {
                _context.Consultations.Remove(consultation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
