using System.Security.Claims;
using LibraryCourse.Data;
using LibraryCourse.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LibraryCourse.Controllers;

public class AccountController(LibraryContext db) : Controller
{
    [HttpGet]
    public IActionResult Login() => View(new LoginModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model, string? returnUrl)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await db.Librarians.SingleOrDefaultAsync(u => u.Login == model.Login.Trim());
        if (user == null || new PasswordHasher<Librarian>().VerifyHashedPassword(user,
            user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, user.Login)],
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction("Index", "Home");
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}