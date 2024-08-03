using DemoPasskeys.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DemoPasskeys.Controllers;

public class UserController(IHttpContextAccessor httpContextAccessor) : Controller
{
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login([FromForm] string email, [FromForm] string password)
    {
        var user = UsersRepository.FindByEmail(email);

        if (user is null || !Password.Verify(password, user.PasswordHash))
        {
            ViewData["email"] = email;
            ViewData["password"] = password;
            ViewData["message"] = "Invalid email address or password";

            Response.StatusCode = 401;
            return View();
        }

        var ctx = httpContextAccessor.HttpContext!;
        ctx.AppendAuthCookie(user.Id);

        return RedirectToAction("Profile");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        httpContextAccessor.HttpContext!.DeleteAuthCookie();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var userId = HttpContext.GetAuthCookie() ?? string.Empty;
        var user = UsersRepository.Read(userId);

        if (user is null)
        {
            Response.StatusCode = 401;
            return RedirectToAction("Login");
        }

        return View(user);
    }
}