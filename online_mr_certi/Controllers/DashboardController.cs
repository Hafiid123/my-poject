using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_mr_certi.Data;
using online_mr_certi.Filters;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;

namespace online_mr_certi.Controllers;

[RequireLogin]
[RequireUserRole]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
        var apps = await _db.MarriageApplications.AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync();

        ViewBag.PendingPayment = apps.Count(a => a.Status == ApplicationStatus.PendingPayment);
        ViewBag.Pending = apps.Count(a => a.Status == ApplicationStatus.Pending);
        ViewBag.Approved = apps.Count(a => a.Status == ApplicationStatus.Approved);
        ViewBag.Rejected = apps.Count(a => a.Status == ApplicationStatus.Rejected);
        return View();
    }
}
