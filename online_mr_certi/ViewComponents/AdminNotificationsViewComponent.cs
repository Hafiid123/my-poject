using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_mr_certi.Data;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;
using online_mr_certi.Models.ViewModels;

namespace online_mr_certi.ViewComponents;

public class AdminNotificationsViewComponent : ViewComponent
{
    private readonly AppDbContext _db;

    public AdminNotificationsViewComponent(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
        if (userId is null)
            return View(new AdminNotificationsViewModel());

        var now = DateTime.UtcNow;
        var overdueThreshold = now.AddHours(-24);

        var readState = await _db.AdminNotificationReadStates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId.Value);
        var lastReadAt = readState?.LastReadAt ?? DateTime.MinValue;

        var alerts = new List<AdminNotificationItemViewModel>();

        var overduePending = await _db.MarriageApplications.AsNoTracking()
            .CountAsync(a =>
                (a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.PendingPayment) &&
                a.SubmissionDate < overdueThreshold);

        if (overduePending > 0)
        {
            alerts.Add(new AdminNotificationItemViewModel
            {
                IsAlert = true,
                IconClass = "fas fa-exclamation-triangle text-danger",
                Message = $"{overduePending} applications pending over 24 hours",
                CreatedAt = now,
                Url = "/Admin/Applications?status=Pending"
            });

            alerts.Add(new AdminNotificationItemViewModel
            {
                IsAlert = true,
                IconClass = "fas fa-radiation text-warning",
                Message = "System requires attention",
                CreatedAt = now.AddSeconds(-1),
                Url = "/Admin/Applications?status=Pending"
            });
        }

        var submitEvents = await _db.MarriageApplications.AsNoTracking()
            .Include(a => a.User)
            .OrderByDescending(a => a.SubmissionDate)
            .Take(8)
            .Select(a => new AdminNotificationItemViewModel
            {
                IconClass = "fas fa-file-upload text-primary",
                Message = $"User {a.User.Name} submitted application #{a.Id}",
                CreatedAt = a.SubmissionDate,
                Url = $"/Admin/ApplicationDetails/{a.Id}"
            })
            .ToListAsync();

        var decisionEvents = await _db.MarriageApplications.AsNoTracking()
            .Where(a => a.DecisionDate != null &&
                        (a.Status == ApplicationStatus.Approved || a.Status == ApplicationStatus.Rejected))
            .OrderByDescending(a => a.DecisionDate)
            .Take(8)
            .Select(a => new AdminNotificationItemViewModel
            {
                IconClass = a.Status == ApplicationStatus.Approved
                    ? "fas fa-check-circle text-success"
                    : "fas fa-times-circle text-danger",
                Message = a.Status == ApplicationStatus.Approved
                    ? $"Admin approved application #{a.Id}"
                    : $"Admin rejected application #{a.Id}",
                CreatedAt = a.DecisionDate!.Value,
                Url = $"/Admin/ApplicationDetails/{a.Id}"
            })
            .ToListAsync();

        var notifications = submitEvents
            .Concat(decisionEvents)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();

        var unreadCount = alerts.Count(a => a.CreatedAt > lastReadAt)
            + notifications.Count(n => n.CreatedAt > lastReadAt);

        return View(new AdminNotificationsViewModel
        {
            UnreadCount = unreadCount,
            Alerts = alerts.OrderByDescending(x => x.CreatedAt).ToList(),
            Notifications = notifications
        });
    }
}

