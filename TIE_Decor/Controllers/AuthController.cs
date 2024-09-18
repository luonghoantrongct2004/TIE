using Microsoft.AspNetCore.Mvc;

namespace TIE_Decor.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
