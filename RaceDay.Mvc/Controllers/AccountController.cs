using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.DTOs;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly RaceDayApiClient _api;
    public AccountController(RaceDayApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var res = await _api.LoginAsync(dto);
        if (res == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(dto);
        }

        // Save Auth State in Session
        HttpContext.Session.SetString("JwtToken", res.Token);
        HttpContext.Session.SetString("UserRole", res.Role);
        HttpContext.Session.SetString("FullName", res.FullName);

        return res.Role == "Organiser"
            ? RedirectToAction("Index", "Organiser")
            : RedirectToAction("Index", "Participant");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
