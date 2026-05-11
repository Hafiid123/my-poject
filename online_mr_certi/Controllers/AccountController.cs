using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_mr_certi.Data;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;
using online_mr_certi.Models.ViewModels;

namespace online_mr_certi.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;

    public AccountController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) is not null)
            return RedirectToLanding();
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) is not null)
            return RedirectToLanding();

        if (!ModelState.IsValid)
            return View(model);

        if (await _db.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
            return View(model);
        }

        var user = new User
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim().ToLowerInvariant(),
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = AppRoles.User,
            RoleId = await _db.Roles
                .Where(r => r.Name == AppRoles.User)
                .Select(r => (int?)r.Id)
                .FirstOrDefaultAsync()
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        TempData["Message"] = "Registration successful. Please sign in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) is not null)
            return RedirectToLanding();
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId) is not null)
            return RedirectToLanding();

        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
        HttpContext.Session.SetString(SessionKeys.UserName, user.Name);
        HttpContext.Session.SetString(SessionKeys.UserEmail, user.Email);
        HttpContext.Session.SetString(SessionKeys.Role, user.Role);
        if (user.RoleId is not null)
            HttpContext.Session.SetInt32(SessionKeys.RoleId, user.RoleId.Value);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return user.Role == AppRoles.Admin
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLanding()
    {
        var role = HttpContext.Session.GetString(SessionKeys.Role);
        // Admin + Staff go to Admin panel; normal users go to User portal dashboard.
        return role == AppRoles.User
            ? RedirectToAction("Index", "Dashboard")
            : RedirectToAction("Index", "Admin");
    }
}
