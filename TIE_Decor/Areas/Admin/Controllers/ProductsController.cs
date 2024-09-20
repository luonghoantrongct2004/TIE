﻿using System;
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
            return View(await _context.Products.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
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
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Description,Year")] Product product, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Danh sách để lưu đường dẫn các ảnh đã upload
                    var imageUrls = new List<string>();

                // Thư mục lưu trữ ảnh
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

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
                product.ImageUrl = imageUrls;

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
        public async Task<IActionResult> Edit([Bind("ProductId,ProductName,Price,Description,Year,CategoryId,BrandId")] Product product, List<IFormFile> images)
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
                    existingProduct.Category = product.Category;
                    existingProduct.Brand = product.Brand;

                    // Nếu có ảnh mới được tải lên
                    if (images != null && images.Count > 0)
                    {
                        // Danh sách để lưu đường dẫn các ảnh mới upload
                        var imageUrls = new List<string>();

                        // Thư mục lưu trữ ảnh
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

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

                        // Gán danh sách đường dẫn ảnh mới vào sản phẩm
                        existingProduct.ImageUrl = imageUrls;
                    }

                    // Cập nhật sản phẩm trong cơ sở dữ liệu
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Product updated successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred: " + ex.Message });
                }
            }

            ViewData["Categories"] = _context.Categories.ToList();
            ViewData["Brands"] = _context.Brands.ToList();

            return Json(new { success = false, message = "Product update false!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);

                // Nếu cần, cũng có thể xóa các tệp ảnh đã upload:
                if (product.ImageUrl != null)
                {
                    foreach (var imageUrl in product.ImageUrl)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
