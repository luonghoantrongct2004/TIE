using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;

namespace TIE_Decor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                .Include(c => c.Category)
                .Include(b => b.Brand)
                .Include(r => r.Reviews)
                .ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products

                .Include(c => c.Category)
                .Include(b => b.Brand)
                .Include(r => r.Reviews)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = _context.Categories.ToList();

            ViewData["Brands"] = _context.Brands.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Danh sách để lưu đường dẫn các ảnh đã upload
                    var imageUrls = new List<string>();

                    // Thư mục lưu trữ ảnh
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // Kiểm tra và tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Xử lý các tệp ảnh được tải lên
                    foreach (var image in images)
                    {
                        if (image != null && image.Length > 0)
                        {
                            // Tạo tên file duy nhất cho từng ảnh để tránh trùng lặp
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(uploadPath, fileName);

                            // Lưu ảnh vào thư mục
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            // Thêm đường dẫn ảnh vào danh sách
                            imageUrls.Add("/uploads/" + fileName);
                        }
                    }

                    // Gán danh sách đường dẫn ảnh vào product
                    product.ImageUrl = string.Join(",", imageUrls);

                    // Thêm sản phẩm vào cơ sở dữ liệu
                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Product created successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred: " + ex.Message });
                }
            }

            ViewData["Categories"] = _context.Categories.ToList();
            ViewData["Brands"] = _context.Brands.ToList();
            return Json(new { success = false, message = "Model validation failed." });
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            ViewData["Categories"] = _context.Categories.ToList();
            ViewData["Brands"] = _context.Brands.ToList();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, List<IFormFile> images)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);

            if (existingProduct == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật các trường khác của sản phẩm
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.Price = product.Price;
                    existingProduct.Description = product.Description;
                    existingProduct.Year = product.Year;
                    existingProduct.CategoryId = product.CategoryId;  // Sửa đúng CategoryId
                    existingProduct.BrandId = product.BrandId;        // Sửa đúng BrandId

                    // Nếu có ảnh mới được tải lên
                    if (images != null && images.Count > 0)
                    {
                        var imageUrls = new List<string>();
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        foreach (var image in images)
                        {
                            if (image != null && image.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                                var filePath = Path.Combine(uploadPath, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                imageUrls.Add("/uploads/" + fileName);
                            }
                        }

                        existingProduct.ImageUrl = string.Join(",", imageUrls);
                    }

                    // Cập nhật sản phẩm trong cơ sở dữ liệu
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    return Redirect("/admin/products");
                }
                catch (Exception ex)
                {
                    return View(product);
                }
            }

            ViewData["Categories"] = _context.Categories.ToList();
            ViewData["Brands"] = _context.Brands.ToList();

            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Remove the product from the database
                _context.Products.Remove(product);

                // Check if there are any image URLs to delete
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    // Split the comma-separated image URLs into a list
                    var imageUrls = product.ImageUrl.Split(',');

                    // Delete each image file
                    foreach (var imageUrl in imageUrls)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            return Redirect("/admin/products");
        }



        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
