using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using TIE_Decor.DbContext;
using TIE_Decor.Entities;
using TIE_Decor.Models;

namespace TIE_Decor.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly AppDbContext _context;
    private readonly StripeSettings _stripeSettings;

    public CheckoutController(AppDbContext context, IOptions<StripeSettings> stripeSettings)
    {
        _context = context;
        _stripeSettings = stripeSettings.Value;
    }

    public IActionResult Checkout()
    {
        var userId = GetCurrentUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == userId.ToString());

        var cartItems = _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToList();

        var checkoutViewModel = new CheckoutViewModel
        {
            User = user,
            CartItems = cartItems,
            TotalPrice = cartItems.Sum(item => item.Product.Price * item.Quantity),
            ShippingAddress = user.ShippingAddress,
            ContactPhone = user.ContactPhone
        };

        return View(checkoutViewModel);
    }

    [HttpPost]
    public IActionResult ProcessPayment(CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Redirect("/checkout/checkout");
        }

        // Lấy lại thông tin người dùng từ cơ sở dữ liệu
        var user = _context.Users.FirstOrDefault(u => u.Id == GetCurrentUserId().ToString());

        if (user != null)
        {
            // Cập nhật địa chỉ giao hàng và số điện thoại
            user.ShippingAddress = model.ShippingAddress;
            user.ContactPhone = model.ContactPhone;
            _context.SaveChanges();

            // Lấy lại giỏ hàng của người dùng từ cơ sở dữ liệu
            var cartItems = _context.Carts
                .Where(c => c.UserId == Guid.Parse(user.Id))
                .Include(c => c.Product)
                .ToList();

            if (!cartItems.Any())
            {
                // Nếu giỏ hàng rỗng, quay lại trang giỏ hàng hoặc hiển thị thông báo
                return Redirect("/cart");
            }

            // Tạo URL đầy đủ cho SuccessUrl và CancelUrl
            var successUrl = $"{Request.Scheme}://{Request.Host}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/checkout";

            // Tạo phiên thanh toán Stripe
            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cartItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = item.Product.Price * 100, // Stripe tính theo cent
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ProductName
                        }
                    },
                    Quantity = item.Quantity
                }).ToList(),
                Mode = "payment",
                SuccessUrl = successUrl,  // Sử dụng URL đầy đủ cho SuccessUrl
                CancelUrl = cancelUrl     // Sử dụng URL đầy đủ cho CancelUrl
            };

            var sessionService = new SessionService();
            Session session = sessionService.Create(sessionOptions);

            // Trả về phiên thanh toán và chuyển hướng đến Stripe Checkout
            return Redirect(session.Url);
        }

        // Nếu không tìm thấy người dùng, quay lại trang checkout
        return Redirect("/checkout/checkout");
    }


    private void SaveOrder(string userId, CheckoutViewModel model)
    {
        var order = new Order
        {
            UserId = Guid.Parse(userId),
            OrderDate = DateTime.Now,
            ShippingAddress = model.ShippingAddress,
            ContactPhone = model.ContactPhone,
            TotalPrice = model.TotalPrice,
            Status = "Paid", // Đơn hàng được đánh dấu là đã thanh toán
            PaymentMethod = "Card",
            PaymentDate = DateTime.Now,
            CreatedAt = DateTime.Now
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        foreach (var item in model.CartItems)
        {
            var orderDetail = new Orderdetail
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                CreatedAt = DateTime.Now
            };

            _context.Orderdetails.Add(orderDetail);
        }

        _context.SaveChanges();
    }


    public IActionResult Success()
    {
        // Lấy ID của phiên thanh toán từ query string
        var sessionId = Request.Query["session_id"].ToString();

        if (string.IsNullOrEmpty(sessionId))
        {
            return RedirectToAction("Checkout");
        }

        // Lấy lại phiên thanh toán từ Stripe để xác nhận
        var sessionService = new SessionService();
        var session = sessionService.Get(sessionId);

        if (session.PaymentStatus == "paid")
        {
            // Lấy lại thông tin người dùng từ cơ sở dữ liệu
            var userId = GetCurrentUserId().ToString();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            // Lấy lại giỏ hàng từ cơ sở dữ liệu
            var cartItems = _context.Carts
                .Where(c => c.UserId == Guid.Parse(user.Id))
                .Include(c => c.Product)
                .ToList();

            // Tạo đối tượng CheckoutViewModel
            var checkoutViewModel = new CheckoutViewModel
            {
                User = user,
                CartItems = cartItems,
                ShippingAddress = user.ShippingAddress,
                ContactPhone = user.ContactPhone,
                TotalPrice = cartItems.Sum(item => item.Product.Price * item.Quantity)
            };

            // Gọi phương thức lưu đơn hàng vào cơ sở dữ liệu
            SaveOrder(userId, checkoutViewModel);

            // Sau khi lưu đơn hàng, xóa giỏ hàng của người dùng
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            // Trả về trang thành công
            return View();
        }

        return Redirect("/checkout/checkout");
    }



    private Guid GetCurrentUserId()
    {
        return new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
