using Microsoft.AspNetCore.Mvc;

namespace TIE_Decor.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
