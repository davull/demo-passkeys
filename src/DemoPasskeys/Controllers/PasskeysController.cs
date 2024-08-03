using Microsoft.AspNetCore.Mvc;

namespace DemoPasskeys.Controllers;

public class PasskeysController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}