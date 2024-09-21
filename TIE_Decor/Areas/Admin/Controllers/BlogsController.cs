using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;
using TIE_Decor.Models;

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class BlogsController : Controller
    {
        private readonly AppDbContext _context;

        public BlogsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Blogs
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            // Tổng số blog
            var totalBlogs = await _context.Blog.CountAsync();

            // Lấy danh sách blog cho trang hiện tại
            var blogs = await _context.Blog
                .OrderByDescending(b => b.CreatedDate)  // Sắp xếp blog mới nhất lên đầu
                .Skip((page - 1) * pageSize)  // Bỏ qua các blog của các trang trước
                .Take(pageSize)  // Lấy số lượng blog cần cho trang hiện tại
                .ToListAsync();

            // Tạo đối tượng chứa thông tin cần thiết cho view
            var model = new BlogViewModel
            {
                Blogs = blogs,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalBlogs / pageSize)
            };

            return View(model);
        }

        // GET: Admin/Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return Redirect("/admin/blogs");
        }

        // GET: Admin/Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Blogs/Create
        [HttpPost]
        public async Task<IActionResult> Create(Blog blog, IFormFile image)
        {
            blog.CreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    // Đường dẫn thư mục upload ảnh
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/blog");

                    // Kiểm tra nếu thư mục không tồn tại thì tạo mới
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Tạo tên file duy nhất để tránh trùng lặp
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Lưu file vào thư mục /blog
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Gán đường dẫn ảnh cho blog
                    blog.ImageUrl = "/blog/" + fileName;
                }
                // Thêm blog vào cơ sở dữ liệu
                _context.Add(blog);
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Blog created successfully!" });
            }

            // Trả về lỗi nếu ModelState không hợp lệ
            return Json(new { success = false, message = "Model validation failed." });
        }


        // GET: Admin/Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Admin/Blogs/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Blog blog, IFormFile image)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu có ảnh được tải lên
                    if (image != null && image.Length > 0)
                    {
                        // Đường dẫn thư mục upload ảnh
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/blog");

                        // Kiểm tra nếu thư mục không tồn tại thì tạo mới
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Tạo tên file duy nhất để tránh trùng lặp
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);

                        // Lưu file vào thư mục /blog
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh mới cho blog
                        blog.ImageUrl = "/blog/" + fileName;
                    }

                    // Cập nhật blog trong cơ sở dữ liệu
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/admin/blogs");
            }
            return View(blog);
        }

        // POST: Admin/Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blog.FindAsync(id);

            if (blog != null)
            {
                // Kiểm tra nếu blog có ảnh
                if (!string.IsNullOrEmpty(blog.ImageUrl))
                {
                    // Lấy đường dẫn tuyệt đối của ảnh trong thư mục /blog
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", blog.ImageUrl.TrimStart('/'));

                    // Kiểm tra nếu file tồn tại, thì xóa file
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Xóa blog từ cơ sở dữ liệu
                _context.Blog.Remove(blog);

                // Lưu các thay đổi
                await _context.SaveChangesAsync();
            }

            // Chuyển hướng về danh sách blogs
            return Redirect("/admin/blogs");
        }

        private bool BlogExists(int id)
        {
            return _context.Blog.Any(e => e.Id == id);
        }
    }
}
