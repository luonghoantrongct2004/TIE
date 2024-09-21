//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TIE_Decor.DbContext;
//using TIE_Decor.Entities;

//namespace TIE_Decor.Controllers
//{
//    public class CartController : Controller
//    {
//        private readonly AppDbContext _db;

//        public CartController(AppDbContext db)
//        {
//            _db = db;
//        }

//        public async Task<IActionResult> Index()
//        {
//            return View(await _db.Carts.ToListAsync());
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("")])
//        {

//        }
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;

namespace TIE_Decor.Controllers;
public class CartController : Controller
{
    private readonly AppDbContext _context;
    private readonly SignInManager<User> _signInManager;

    public CartController(AppDbContext context, SignInManager<User> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }

    // Add product to cart
    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, Guid userId)
    {
        var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
        if (cartItem != null)
        {
            // Update quantity if item already exists
            cartItem.Quantity++;
        }
        else
        {
            // Create new cart item
            cartItem = new Cart { ProductId = productId, UserId = userId, Quantity = 1 };
            _context.Carts.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // Increase quantity
    public async Task<IActionResult> IncreaseQuantity(int cartId)
    {
        var cartItem = await _context.Carts.FindAsync(cartId);
        if (cartItem == null)
        {
            return NotFound();
        }

        cartItem.Quantity++;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // Decrease quantity
    public async Task<IActionResult> DecreaseQuantity(int cartId)
    {
        var cartItem = await _context.Carts.FindAsync(cartId);
        if (cartItem == null)
        {
            return NotFound();
        }

        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    // Remove item from cart
    public async Task<IActionResult> Delete(int cartId)
    {
        var cartItem = await _context.Carts.FindAsync(cartId);
        if (cartItem != null)
        {
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    // Display cart
    public async Task<IActionResult> Index()
    {
        //if (!_signInManager.IsSignedIn(User))
        if (!User.Identity.IsAuthenticated)
        {
            return Redirect("/Auth/Login");
        }

        Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var cartItems = await _context.Carts
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        decimal totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);
        ViewData["TotalAmount"] = totalAmount;

        return View(cartItems);
    }
}
