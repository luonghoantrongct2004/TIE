using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConsultationManageController : Controller
    {
        private readonly AppDbContext _context;

        public ConsultationManageController(AppDbContext context)
        {
            _context = context;
        }

     
        public async Task<IActionResult> Index()
        {

            var designerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(designerIdClaim, out Guid designerIdGuid))
            {
                return BadRequest("Invalid DesignerId.");
            }
           
            var consultations = await _context.Consultations
                .Include(c => c.User)
                .Where(c => c.DesignerID == designerIdGuid)
                .ToListAsync();

            return View(consultations);
        }

       
        [HttpPost]
        [Route("Admin/ConsultationManage/Confirm/{id:int}")]
        public async Task<IActionResult> Confirm(int id)
        {
           
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound("Buổi tư vấn không tồn tại.");
            }

         
            consultation.Status = "Confirmed";
            await _context.SaveChangesAsync();

         
            return Redirect("/admin/consultationmanage");
        }
        
        [HttpPost]
        [Route("Admin/ConsultationManage/Decline/{id:int}")]
        public async Task<IActionResult> Decline(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound("Buổi tư vấn không tồn tại.");
            }

         
            consultation.Status = "Declined";
            await _context.SaveChangesAsync();
            return Redirect("/admin/consultationmanage");
        }

       

        public async Task<IActionResult> Delete(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound("Buổi tư vấn không tồn tại.");
            }

            _context.Consultations.Remove(consultation);
            await _context.SaveChangesAsync();

            return Redirect("/admin/consultationmanage");
        }

        

    }
}
