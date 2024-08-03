using Microsoft.AspNetCore.Mvc;

namespace DemoPasskeys.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}