using Microsoft.AspNetCore.Mvc;

namespace TIE_Decor.Areas.Admin.Controllers;

[Area("Admin")]
public class Dashboard : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
