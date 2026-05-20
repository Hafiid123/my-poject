using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_mr_certi.Data;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;
using online_mr_certi.Models.ViewModels;
using System.Net;
using System.Net.Mail;

namespace online_mr_certi.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;

    public AccountController(AppDbContext db)
    {
        _db = db;
    }

    // 1. GET: Account/ForgotPassword
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // 2. POST: Account/ForgotPassword (Halkan ayaa isbadalka rasmiga ah lagu sameeyay!)
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user != null)
        {
            string token = Guid.NewGuid().ToString();

            // Kani waa link-gii dhabta ah ee qofku ku dhufan lahaa
            var resetLink = Url.Action("ResetPassword", "Account",
                new { email = user.Email, token = token }, Request.Scheme);

            try
            {
                var mailMessage = new MailMessage
                {
                    // Halkaan ku qor Email-kaaga rasmiga ah ee nidaamku ka dhex dirayo xogta
                    From = new MailAddress("portal@marriage-registry.com", "Marriage Registry System"),
                    Subject = "Password Reset Request",
                    Body = $@"
                    <div style='font-family: sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 8px; max-width: 500px;'>
                        <h3 style='color: #2563eb;'>Password Reset Request</h3>
                        <p>Waxaad nidaamka guurka ka soo codsatay inaad beddesho password-kaaga.</p>
                        <p>Fadlan guji badanka hoose si aad u cusbooneysiiso xogtaada:</p>
                        <a href='{resetLink}' style='background: #2563eb; color: white; padding: 10px 20px; text-decoration: none; border-radius: 6px; display: inline-block; font-weight: bold;'>Reset Password</a>
                        <br/><br/>
                        <p style='color: #666; font-size: 12px;'>Haddii aadan adigu codsan, fadlan iska indho-tir email-kan.</p>
                    </div>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(user.Email);

                // --- KOODHKA Google Gmail SMTP (REAL EMAIL) ---
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    // 1. Email-kaaga Gmail-ka ah oo ah kan wax diraya:
                    string myGmail = "hoooyo1230@gmail.com";

                    // 2. 16-ka xaraf ee Google App Password (ka soo sameey Google Account):
                    string myAppPassword = "qcej mtoi bbbl ogqv";

                    smtpClient.Credentials = new NetworkCredential(myGmail, myAppPassword);
                    smtpClient.EnableSsl = true;

                    // Hadda wuxuu si toos ah ugu dhacayaa Gmail-ka dhabta ah ee qofka (user.Email)!
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Email-ka lama diri karo hadda. Fadlan dib u tijaabi.");
                return View(model);
            }
        }

        return RedirectToAction("ForgotPasswordConfirmation");
    }

    // 3. GET: Account/ForgotPasswordConfirmation
    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    // 4. GET: Account/ResetPassword (Marka Link-ga Gmail-ka laga soo gujiyo)
    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordViewModel { Email = email, Token = token };
        return View(model);
    }

    // 5. POST: Account/ResetPassword (Kaydinta Password-ka cusub ee Database-ka)
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // Password-ka cusub ayaan si ammaan ah u hash-gareynaynaa
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

        // Waxay si toos ah u beddelaysaa tiirkii 'Password' ee database-kaaga ku jiray
        user.Password = hashedPassword;

        _db.Entry(user).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Password-kaagii si guul leh ayaa loo baddalay!";
        return RedirectToAction("Login");
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
        {
            HttpContext.Session.SetInt32(SessionKeys.RoleId, user.RoleId.Value);

            var permissions = await _db.RolePermissions
                .Where(rp => rp.RoleId == user.RoleId.Value)
                .Join(_db.Permissions, rp => rp.PermissionId, p => p.Id, (rp, p) => p.Name)
                .ToListAsync();

            HttpContext.Session.SetString(SessionKeys.Permissions, string.Join(",", permissions));
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return user.Role == AppRoles.User
            ? RedirectToAction("Index", "Dashboard")
            : RedirectToAction("Index", "Admin");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        //if (HttpContext.Session.GetInt32(SessionKeys.UserId) is not null)
        //    return RedirectToLanding();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLanding()
    {
        var role = HttpContext.Session.GetString(SessionKeys.Role);
        return role == AppRoles.User
            ? RedirectToAction("Index", "Dashboard")
            : RedirectToAction("Index", "Admin");
    }
}