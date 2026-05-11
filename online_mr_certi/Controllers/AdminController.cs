using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_mr_certi.Data;
using online_mr_certi.Filters;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;
using online_mr_certi.Models.ViewModels;
using online_mr_certi.Services;

namespace online_mr_certi.Controllers;

[RequireAdminPanel]
public class AdminController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly ICertificatePdfService _pdf;

    public AdminController(AppDbContext db, IWebHostEnvironment env, ICertificatePdfService pdf)
    {
        _db = db;
        _env = env;
        _pdf = pdf;
    }

    [RequirePermission(AppPermissions.ViewDashboard)]
    public async Task<IActionResult> Index(string? q, string? status)
    {
        var now = DateTime.UtcNow;
        var startDate = now.Date.AddDays(-6);

        var allApps = _db.MarriageApplications.AsNoTracking();

        var totalApplications = await allApps.CountAsync();
        var pendingPayment = await allApps.CountAsync(a => a.Status == ApplicationStatus.PendingPayment);
        var pending = await allApps.CountAsync(a => a.Status == ApplicationStatus.Pending);
        var approved = await allApps.CountAsync(a => a.Status == ApplicationStatus.Approved);
        var rejected = await allApps.CountAsync(a => a.Status == ApplicationStatus.Rejected);

        var trendRaw = await allApps
            .Where(a => a.SubmissionDate >= startDate)
            .GroupBy(a => a.SubmissionDate.Date)
            .Select(g => new { Day = g.Key, Count = g.Count() })
            .ToListAsync();
        var trendMap = trendRaw.ToDictionary(x => x.Day, x => x.Count);
        var trendLabels = Enumerable.Range(0, 7)
            .Select(i => startDate.AddDays(i))
            .ToList();

        var decidedApps = await allApps
            .Where(a => a.DecisionDate != null &&
                        (a.Status == ApplicationStatus.Approved || a.Status == ApplicationStatus.Rejected))
            .Select(a => new { a.SubmissionDate, DecisionDate = a.DecisionDate!.Value })
            .ToListAsync();

        var avgProcessingHours = decidedApps.Count == 0
            ? 0d
            : decidedApps
                .Select(a => (a.DecisionDate - a.SubmissionDate).TotalHours)
                .Where(hours => hours >= 0)
                .DefaultIfEmpty(0d)
                .Average();

        var overdueThreshold = now.AddHours(-24);
        var overdueCount = await allApps.CountAsync(a =>
            (a.Status == ApplicationStatus.Pending || a.Status == ApplicationStatus.PendingPayment) &&
            a.SubmissionDate < overdueThreshold);

        var filterQuery = _db.MarriageApplications.AsNoTracking()
            .Include(a => a.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            (status == ApplicationStatus.PendingPayment ||
             status == ApplicationStatus.Pending ||
             status == ApplicationStatus.Approved ||
             status == ApplicationStatus.Rejected))
        {
            filterQuery = filterQuery.Where(a => a.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            filterQuery = filterQuery.Where(a =>
                a.Id.ToString() == term ||
                a.HusbandName.Contains(term) ||
                a.WifeName.Contains(term));
        }

        var filtered = await filterQuery
            .OrderByDescending(a => a.SubmissionDate)
            .Take(20)
            .Select(a => new AdminDashboardApplicationRow
            {
                Id = a.Id,
                ApplicantName = $"{a.HusbandName} & {a.WifeName}",
                ApplicantEmail = a.User.Email,
                Status = a.Status,
                SubmissionDate = a.SubmissionDate
            })
            .ToListAsync();

        var submissionActivities = await _db.MarriageApplications.AsNoTracking()
            .Include(a => a.User)
            .OrderByDescending(a => a.SubmissionDate)
            .Take(5)
            .Select(a => new ActivityItemViewModel
            {
                Message = $"User {a.User.Name} submitted application #{a.Id}",
                CreatedAt = a.SubmissionDate
            })
            .ToListAsync();

        var paymentActivities = await _db.Payments.AsNoTracking()
            .Include(p => p.Application)
            .ThenInclude(a => a.User)
            .Where(p => p.PaymentDate != null &&
                        (p.PaymentStatus == PaymentStatuses.Approved || p.PaymentStatus == PaymentStatuses.Rejected))
            .OrderByDescending(p => p.PaymentDate)
            .Take(5)
            .Select(p => new ActivityItemViewModel
            {
                Message = p.PaymentStatus == PaymentStatuses.Approved
                    ? $"Admin approved payment for application #{p.ApplicationId}"
                    : $"Admin rejected payment for application #{p.ApplicationId}",
                CreatedAt = p.PaymentDate!.Value
            })
            .ToListAsync();

        var activities = submissionActivities
            .Concat(paymentActivities)
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToList();

        var vm = new AdminDashboardViewModel
        {
            TotalApplications = totalApplications,
            PendingPayment = pendingPayment,
            Pending = pending,
            Approved = approved,
            Rejected = rejected,
            TrendLabels = trendLabels.Select(d => d.ToString("ddd")).ToList(),
            TrendValues = trendLabels.Select(d => trendMap.TryGetValue(d, out var cnt) ? cnt : 0).ToList(),
            StatusApproved = approved,
            StatusRejected = rejected,
            StatusPending = pending + pendingPayment,
            AverageProcessingHours = Math.Round(avgProcessingHours, 2),
            OverdueApplicationsCount = overdueCount,
            SearchQuery = q,
            StatusFilter = status,
            FilteredApplications = filtered,
            RecentActivities = activities
        };

        return View(vm);
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ViewDashboard)]
    public IActionResult NotificationBell()
    {
        return ViewComponent("AdminNotifications");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ViewDashboard)]
    public async Task<IActionResult> MarkNotificationsRead()
    {
        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
        if (userId is null)
            return RedirectToAction("Login", "Account");

        var row = await _db.AdminNotificationReadStates.FirstOrDefaultAsync(x => x.UserId == userId.Value);
        if (row is null)
        {
            _db.AdminNotificationReadStates.Add(new AdminNotificationReadState
            {
                UserId = userId.Value,
                LastReadAt = DateTime.UtcNow
            });
        }
        else
        {
            row.LastReadAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return Redirect(Request.Headers.Referer.ToString());
    }

    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> Users()
    {
        var users = await _db.Users.AsNoTracking()
            .OrderByDescending(u => u.Id)
            .ToListAsync();
        return View(users);
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> CreateUser()
    {
        ViewBag.Roles = await _db.Roles.AsNoTracking()
            .Where(r => r.Name != AppRoles.User)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return View(new AdminCreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> CreateUser(AdminCreateUserViewModel model)
    {
        ViewBag.Roles = await _db.Roles.AsNoTracking()
            .Where(r => r.Name != AppRoles.User)
            .OrderBy(r => r.Name)
            .ToListAsync();

        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email already exists.");
            return View(model);
        }

        var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == model.RoleId);
        if (role is null || role.Name == AppRoles.User)
        {
            ModelState.AddModelError(nameof(model.RoleId), "Invalid role.");
            return View(model);
        }

        var rolePermissionNames = await _db.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == role.Id)
            .Join(_db.Permissions.AsNoTracking(),
                rp => rp.PermissionId,
                p => p.Id,
                (rp, p) => p.Name)
            .ToListAsync();

        if (rolePermissionNames.Count == 0)
        {
            ModelState.AddModelError(nameof(model.RoleId), "Selected role has no permissions assigned.");
            return View(model);
        }

        if (!rolePermissionNames.Contains(AppPermissions.ViewDashboard))
        {
            ModelState.AddModelError(nameof(model.RoleId), "Selected role must include ViewDashboard permission.");
            return View(model);
        }

        // Keep existing string-role behavior: only the real system admin uses Role="Admin".
        // Other staff roles still get RoleId for permission checks.
        var roleString = role.Name == AppRoles.Admin ? AppRoles.Admin : "Staff";

        _db.Users.Add(new User
        {
            Name = model.Name.Trim(),
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = roleString,
            RoleId = role.Id,
            PaymentStatus = UserPaymentStatuses.Unpaid
        });
        await _db.SaveChangesAsync();

        TempData["Message"] = "User created successfully.";
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageRoles)]
    public async Task<IActionResult> Roles()
    {
        var roles = await _db.Roles.AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync();
        return View(roles);
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageRoles)]
    public async Task<IActionResult> CreateRole()
    {
        var perms = await _db.Permissions.AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        var vm = new RoleEditViewModel
        {
            Permissions = perms.Select(p => new RolePermissionCheckboxViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Selected = false
            }).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageRoles)]
    public async Task<IActionResult> CreateRole(RoleEditViewModel model)
    {
        var perms = await _db.Permissions.AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        // Rehydrate names for display if model binding only sent Id/Selected
        var byId = perms.ToDictionary(p => p.Id, p => p.Name);
        foreach (var p in model.Permissions)
            p.Name = byId.TryGetValue(p.Id, out var n) ? n : p.Name;

        if (!ModelState.IsValid)
            return View(model);

        var name = model.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(nameof(model.Name), "Role name is required.");
            return View(model);
        }

        if (await _db.Roles.AnyAsync(r => r.Name == name))
        {
            ModelState.AddModelError(nameof(model.Name), "Role name already exists.");
            return View(model);
        }

        var selectedPermissionIds = model.Permissions.Where(p => p.Selected).Select(p => p.Id).Distinct().ToList();
        if (selectedPermissionIds.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Select at least one permission.");
            return View(model);
        }

        var viewDashboardPermissionId = perms
            .Where(p => p.Name == AppPermissions.ViewDashboard)
            .Select(p => p.Id)
            .FirstOrDefault();

        if (viewDashboardPermissionId != 0 && !selectedPermissionIds.Contains(viewDashboardPermissionId))
            selectedPermissionIds.Add(viewDashboardPermissionId);

        var role = new Role { Name = name, CreatedAt = DateTime.UtcNow };
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();

        foreach (var pid in selectedPermissionIds)
            _db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = pid });
        await _db.SaveChangesAsync();

        TempData["Message"] = "Role created successfully.";
        return RedirectToAction(nameof(Roles));
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageRoles)]
    public async Task<IActionResult> EditRole(int id)
    {
        var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if (role is null)
            return NotFound();

        var permIds = await _db.RolePermissions.AsNoTracking()
            .Where(rp => rp.RoleId == id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var perms = await _db.Permissions.AsNoTracking().OrderBy(p => p.Name).ToListAsync();

        var vm = new RoleEditViewModel
        {
            Id = role.Id,
            Name = role.Name,
            Permissions = perms.Select(p => new RolePermissionCheckboxViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Selected = permIds.Contains(p.Id)
            }).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageRoles)]
    public async Task<IActionResult> EditRole(RoleEditViewModel model)
    {
        if (model.Id is null)
            return BadRequest();

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == model.Id.Value);
        if (role is null)
            return NotFound();

        var perms = await _db.Permissions.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
        var byId = perms.ToDictionary(p => p.Id, p => p.Name);
        foreach (var p in model.Permissions)
            p.Name = byId.TryGetValue(p.Id, out var n) ? n : p.Name;

        if (!ModelState.IsValid)
            return View(model);

        var name = model.Name.Trim();
        if (await _db.Roles.AnyAsync(r => r.Name == name && r.Id != role.Id))
        {
            ModelState.AddModelError(nameof(model.Name), "Role name already exists.");
            return View(model);
        }

        role.Name = name;

        var selectedPermissionIds = model.Permissions.Where(p => p.Selected).Select(p => p.Id).Distinct().ToHashSet();
        if (selectedPermissionIds.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Select at least one permission.");
            return View(model);
        }

        var viewDashboardPermissionId = perms
            .Where(p => p.Name == AppPermissions.ViewDashboard)
            .Select(p => p.Id)
            .FirstOrDefault();
        if (viewDashboardPermissionId != 0)
            selectedPermissionIds.Add(viewDashboardPermissionId);
        var existingPermissionIds = await _db.RolePermissions
            .Where(rp => rp.RoleId == role.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var toRemove = existingPermissionIds.Where(pid => !selectedPermissionIds.Contains(pid)).ToList();
        var toAdd = selectedPermissionIds.Where(pid => !existingPermissionIds.Contains(pid)).ToList();

        if (toRemove.Count > 0)
        {
            _db.RolePermissions.RemoveRange(_db.RolePermissions.Where(rp => rp.RoleId == role.Id && toRemove.Contains(rp.PermissionId)));
        }

        foreach (var pid in toAdd)
            _db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = pid });

        await _db.SaveChangesAsync();

        TempData["Message"] = "Role updated successfully.";
        return RedirectToAction(nameof(Roles));
    }

    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> UserDetails(int id)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound();

        ViewBag.ApplicationCount = await _db.MarriageApplications.CountAsync(a => a.UserId == id);
        return View(user);
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound();

        var vm = new AdminUserEditViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> EditUser(AdminUserEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Role != AppRoles.Admin && model.Role != AppRoles.User)
        {
            ModelState.AddModelError(nameof(model.Role), "Invalid role.");
            return View(model);
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (user is null)
            return NotFound();

        var email = model.Email.Trim().ToLowerInvariant();
        var duplicateEmail = await _db.Users.AnyAsync(u => u.Email == email && u.Id != model.Id);
        if (duplicateEmail)
        {
            ModelState.AddModelError(nameof(model.Email), "Email already exists.");
            return View(model);
        }

        user.Name = model.Name.Trim();
        user.Email = email;
        user.Role = model.Role;
        await _db.SaveChangesAsync();

        TempData["Message"] = "User updated successfully.";
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound();

        ViewBag.ApplicationCount = await _db.MarriageApplications.CountAsync(a => a.UserId == id);
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageUsers)]
    public async Task<IActionResult> DeleteUserConfirmed(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return NotFound();

        var currentUserId = HttpContext.Session.GetInt32(SessionKeys.UserId);
        if (currentUserId == user.Id)
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Users));
        }

        var hasApplications = await _db.MarriageApplications.AnyAsync(a => a.UserId == id);
        if (hasApplications)
        {
            TempData["Error"] = "Cannot delete user with existing applications.";
            return RedirectToAction(nameof(Users));
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        TempData["Message"] = "User deleted successfully.";
        return RedirectToAction(nameof(Users));
    }

    [RequirePermission(AppPermissions.ViewApplication)]
    public async Task<IActionResult> Applications(string? q, string? status)
    {
        var query = _db.MarriageApplications.AsNoTracking()
            .Include(a => a.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            (status == ApplicationStatus.PendingPayment ||
             status == ApplicationStatus.Pending ||
             status == ApplicationStatus.Approved ||
             status == ApplicationStatus.Rejected))
            query = query.Where(a => a.Status == status);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(a =>
                a.HusbandName.Contains(term) ||
                a.WifeName.Contains(term) ||
                a.User.Email.Contains(term) ||
                a.Id.ToString() == term ||
                a.Witnesses.Any(w => w.FullName.Contains(term) || w.IdNumber.Contains(term)));
        }

        var list = await query.OrderByDescending(a => a.SubmissionDate).ToListAsync();
        ViewBag.Query = q;
        ViewBag.StatusFilter = status;
        return View(list);
    }
    [RequirePermission(AppPermissions.ViewApplication)]
    public async Task<IActionResult> ApplicationDetails(int id)
    {
        var app = await _db.MarriageApplications
            .Include(a => a.User)
            .Include(a => a.Documents)
            .Include(a => a.Witnesses)
            .Include(a => a.Certificate)
            .Include(a => a.Payment)
            .ThenInclude(p => p!.Fee)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (app is null)
            return NotFound();
        return View(app);
    }

    [RequirePermission(AppPermissions.ManageFees)]
    public async Task<IActionResult> FeesSettings()
    {
        var fees = await _db.Fees.AsNoTracking()
            .OrderBy(f => f.Id)
            .ToListAsync();
        return View(fees);
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageFees)]
    public IActionResult CreateFee()
    {
        return View(new FeeCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageFees)]
    public async Task<IActionResult> CreateFee(FeeCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _db.Fees.Add(new Fee
        {
            ServiceName = model.ServiceName.Trim(),
            Amount = model.Amount,
            Currency = model.Currency.Trim().ToUpperInvariant(),
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        TempData["Message"] = "Fee created.";
        return RedirectToAction(nameof(FeesSettings));
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageFees)]
    public async Task<IActionResult> EditFee(int id)
    {
        var fee = await _db.Fees.FirstOrDefaultAsync(f => f.Id == id);
        if (fee is null)
            return NotFound();

        var vm = new FeeEditViewModel
        {
            Id = fee.Id,
            ServiceName = fee.ServiceName,
            Amount = fee.Amount,
            Currency = fee.Currency,
            IsActive = fee.IsActive
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageFees)]
    public async Task<IActionResult> EditFee(FeeEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var fee = await _db.Fees.FirstOrDefaultAsync(f => f.Id == model.Id);
        if (fee is null)
            return NotFound();

        fee.ServiceName = model.ServiceName.Trim();
        fee.Amount = model.Amount;
        fee.Currency = model.Currency.Trim().ToUpperInvariant();
        fee.IsActive = model.IsActive;
        await _db.SaveChangesAsync();

        TempData["Message"] = "Fee updated.";
        return RedirectToAction(nameof(FeesSettings));
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ManageFees)]
    public async Task<IActionResult> DeleteFee(int id)
    {
        var fee = await _db.Fees.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (fee is null)
            return NotFound();

        ViewBag.PaymentsReferencingFee = await _db.Payments.CountAsync(p => p.FeeId == id);
        return View(fee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManageFees)]
    public async Task<IActionResult> DeleteFeeConfirmed(int id)
    {
        var fee = await _db.Fees.FirstOrDefaultAsync(f => f.Id == id);
        if (fee is null)
            return NotFound();

        var referenced = await _db.Payments.AnyAsync(p => p.FeeId == id);
        if (referenced)
        {
            TempData["Error"] =
                "This fee cannot be deleted because payment records still reference it. Set the fee to inactive instead, or contact support.";
            return RedirectToAction(nameof(FeesSettings));
        }

        _db.Fees.Remove(fee);
        await _db.SaveChangesAsync();

        TempData["Message"] = "Fee deleted.";
        return RedirectToAction(nameof(FeesSettings));
    }

    [RequirePermission(AppPermissions.ManagePayments)]
    public async Task<IActionResult> Payments(string? status)
    {
        var query = _db.Payments.AsNoTracking()
            .Include(p => p.Application)
            .ThenInclude(a => a.User)
            .Include(p => p.Fee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            (status == PaymentStatuses.Pending ||
             status == PaymentStatuses.Approved ||
             status == PaymentStatuses.Rejected))
            query = query.Where(p => p.PaymentStatus == status);

        var list = await query.OrderByDescending(p => p.Id).ToListAsync();
        ViewBag.StatusFilter = status;
        return View(list);
    }

    [RequirePermission(AppPermissions.ManagePayments)]
    public async Task<IActionResult> PaymentDetails(int id)
    {
        var payment = await _db.Payments
            .Include(p => p.Application)
            .ThenInclude(a => a.User)
            .Include(p => p.Fee)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (payment is null)
            return NotFound();
        return View(payment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManagePayments)]
    public async Task<IActionResult> ApprovePayment(int id)
    {
        var payment = await _db.Payments
            .Include(p => p.Application)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (payment is null)
            return NotFound();

        payment.PaymentStatus = PaymentStatuses.Approved;
        payment.PaymentDate = DateTime.UtcNow;

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == payment.UserId);
        if (user is not null)
            user.PaymentStatus = UserPaymentStatuses.Paid;

        if (payment.Application.Status == ApplicationStatus.PendingPayment)
            payment.Application.Status = ApplicationStatus.Pending;

        await _db.SaveChangesAsync();

        TempData["Message"] = "Payment approved. The application can now be reviewed.";
        return RedirectToAction(nameof(PaymentDetails), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ManagePayments)]
    public async Task<IActionResult> RejectPayment(int id)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Id == id);
        if (payment is null)
            return NotFound();

        payment.PaymentStatus = PaymentStatuses.Rejected;
        payment.PaymentDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Message"] = "Payment rejected. The applicant may submit new payment details.";
        return RedirectToAction(nameof(PaymentDetails), new { id });
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ApproveApplications)]
    public async Task<IActionResult> EditApplication(int id)
    {
        var app = await _db.MarriageApplications.FirstOrDefaultAsync(a => a.Id == id);
        if (app is null)
            return NotFound();

        var vm = new AdminApplicationEditViewModel
        {
            Id = app.Id,
            HusbandName = app.HusbandName,
            HusbandDob = app.HusbandDob,
            HusbandIdNumber = app.HusbandIdNumber,
            HusbandContactNumber = app.HusbandContactNumber,
            HusbandAddress = app.HusbandAddress,
            WifeName = app.WifeName,
            WifeDob = app.WifeDob,
            WifeIdNumber = app.WifeIdNumber,
            WifeContactNumber = app.WifeContactNumber,
            WifeAddress = app.WifeAddress,
            MarriageDate = app.MarriageDate,
            MarriageLocation = app.MarriageLocation,
            Status = app.Status,
            Remarks = app.Remarks
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ApproveApplications)]
    public async Task<IActionResult> EditApplication(AdminApplicationEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Status != ApplicationStatus.PendingPayment &&
            model.Status != ApplicationStatus.Pending &&
            model.Status != ApplicationStatus.Approved &&
            model.Status != ApplicationStatus.Rejected)
        {
            ModelState.AddModelError(nameof(model.Status), "Invalid status.");
            return View(model);
        }

        var app = await _db.MarriageApplications
            .Include(a => a.Certificate)
            .FirstOrDefaultAsync(a => a.Id == model.Id);
        if (app is null)
            return NotFound();

        app.HusbandName = model.HusbandName.Trim();
        app.HusbandDob = model.HusbandDob.Date;
        app.HusbandIdNumber = model.HusbandIdNumber.Trim();
        app.HusbandContactNumber = model.HusbandContactNumber.Trim();
        app.HusbandAddress = model.HusbandAddress.Trim();
        app.WifeName = model.WifeName.Trim();
        app.WifeDob = model.WifeDob.Date;
        app.WifeIdNumber = model.WifeIdNumber.Trim();
        app.WifeContactNumber = model.WifeContactNumber.Trim();
        app.WifeAddress = model.WifeAddress.Trim();
        app.MarriageDate = model.MarriageDate.Date;
        app.MarriageLocation = model.MarriageLocation.Trim();
        app.Status = model.Status;
        app.Remarks = string.IsNullOrWhiteSpace(model.Remarks) ? null : model.Remarks.Trim();
        app.DecisionDate = model.Status == ApplicationStatus.Approved || model.Status == ApplicationStatus.Rejected
            ? DateTime.UtcNow
            : null;

        if (model.Status == ApplicationStatus.Approved)
        {
            var paid = await _db.Payments.AsNoTracking()
                .AnyAsync(p => p.ApplicationId == model.Id && p.PaymentStatus == PaymentStatuses.Approved);
            if (!paid)
            {
                ModelState.AddModelError(nameof(model.Status),
                    "Cannot approve until the fee payment is verified (Approved).");
                return View(model);
            }
        }

        await _db.SaveChangesAsync();

        if (app.Status == ApplicationStatus.Approved)
        {
            if (app.Certificate is not null)
            {
                var oldPath = Path.Combine(_env.WebRootPath, app.Certificate.CertificateFile.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
                _db.Certificates.Remove(app.Certificate);
                await _db.SaveChangesAsync();
            }

            await EnsureCertificateAsync(app);
            await _db.SaveChangesAsync();
        }

        TempData["Message"] = "Application updated successfully.";
        return RedirectToAction(nameof(ApplicationDetails), new { id = app.Id });
    }

    [HttpGet]
    [RequirePermission(AppPermissions.ApproveApplications)]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var app = await _db.MarriageApplications
            .AsNoTracking()
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (app is null)
            return NotFound();

        return View(app);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ApproveApplications)]
    public async Task<IActionResult> DeleteApplicationConfirmed(int id)
    {
        var app = await _db.MarriageApplications
            .Include(a => a.Documents)
            .Include(a => a.Certificate)
            .Include(a => a.Payment)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (app is null)
            return NotFound();

        foreach (var doc in app.Documents)
        {
            var path = Path.Combine(_env.WebRootPath, doc.FilePath.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        if (app.Certificate is not null)
        {
            var certPath = Path.Combine(_env.WebRootPath, app.Certificate.CertificateFile.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(certPath))
                System.IO.File.Delete(certPath);
        }

        if (app.Payment is not null && !string.IsNullOrEmpty(app.Payment.ReceiptImage))
        {
            var receiptPath = Path.Combine(_env.WebRootPath, app.Payment.ReceiptImage.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(receiptPath))
                System.IO.File.Delete(receiptPath);
        }

        _db.MarriageApplications.Remove(app);
        await _db.SaveChangesAsync();

        TempData["Message"] = "Application deleted successfully.";
        return RedirectToAction(nameof(Applications));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.ApproveApplications)]
    public async Task<IActionResult> Approve(int id)
    {
        var app = await _db.MarriageApplications
            .Include(a => a.Certificate)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (app is null)
            return NotFound();

        var paid = await _db.Payments.AsNoTracking()
            .AnyAsync(p => p.ApplicationId == id && p.PaymentStatus == PaymentStatuses.Approved);
        if (!paid)
        {
            TempData["Error"] = "Cannot approve until the fee payment is verified (Approved).";
            return RedirectToAction(nameof(ApplicationDetails), new { id });
        }

        app.Status = ApplicationStatus.Approved;
        app.Remarks = null;
        app.DecisionDate = DateTime.UtcNow;
        await EnsureCertificateAsync(app);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Application approved and certificate generated.";
        return RedirectToAction(nameof(ApplicationDetails), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.RejectApplications)]
    public async Task<IActionResult> Reject(AdminRejectViewModel model)
    {
        var app = await _db.MarriageApplications.FirstOrDefaultAsync(a => a.Id == model.Id);
        if (app is null)
            return NotFound();

        app.Status = ApplicationStatus.Rejected;
        app.Remarks = string.IsNullOrWhiteSpace(model.Remarks) ? null : model.Remarks.Trim();
        app.DecisionDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Message"] = "Application rejected.";
        return RedirectToAction(nameof(ApplicationDetails), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(AppPermissions.IssueCertificate)]
    public async Task<IActionResult> RegenerateCertificate(int id)
    {
        var app = await _db.MarriageApplications
            .Include(a => a.Certificate)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (app is null || app.Status != ApplicationStatus.Approved)
            return NotFound();

        if (app.Certificate is not null)
        {
            var oldPath = Path.Combine(_env.WebRootPath, app.Certificate.CertificateFile.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            _db.Certificates.Remove(app.Certificate);
            await _db.SaveChangesAsync();
        }

        await EnsureCertificateAsync(app);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Certificate regenerated.";
        return RedirectToAction(nameof(ApplicationDetails), new { id });
    }

    [RequirePermission(AppPermissions.ViewDashboard)]
    public async Task<IActionResult> Reports()
    {
        var apps = await _db.MarriageApplications.AsNoTracking()
            .Include(a => a.User)
            .OrderByDescending(a => a.SubmissionDate)
            .Take(100)
            .ToListAsync();

        var allForStats = await _db.MarriageApplications.AsNoTracking().ToListAsync();
        var byMonth = allForStats
            .GroupBy(a => new { a.SubmissionDate.Year, a.SubmissionDate.Month })
            .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
            .Take(12)
            .Select(g => new MonthlyReportRow
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                Total = g.Count(),
                Pending = g.Count(x => x.Status == ApplicationStatus.PendingPayment || x.Status == ApplicationStatus.Pending),
                Approved = g.Count(x => x.Status == ApplicationStatus.Approved),
                Rejected = g.Count(x => x.Status == ApplicationStatus.Rejected)
            })
            .ToList();

        var vm = new ReportsViewModel
        {
            ByMonth = byMonth,
            RecentApplications = apps.Select(a => new MarriageApplicationSummary
            {
                Id = a.Id,
                ApplicantEmail = a.User.Email,
                HusbandName = a.HusbandName,
                WifeName = a.WifeName,
                Status = a.Status,
                SubmissionDate = a.SubmissionDate
            }).ToList()
        };

        return View(vm);
    }

    private async Task EnsureCertificateAsync(MarriageApplication app)
    {
        if (await _db.Certificates.AnyAsync(c => c.ApplicationId == app.Id))
            return;

        var appForPdf = await _db.MarriageApplications
            .AsNoTracking()
            .Include(a => a.Witnesses)
            .FirstAsync(a => a.Id == app.Id);

        var dir = Path.Combine(_env.WebRootPath, "uploads", "certificates");
        Directory.CreateDirectory(dir);
        var fileName = $"{app.Id}_{Guid.NewGuid():N}.pdf";
        var relative = Path.Combine("uploads", "certificates", fileName).Replace('\\', '/');
        var physical = Path.Combine(dir, fileName);

        await using (var fs = System.IO.File.Create(physical))
            _pdf.GenerateCertificatePdf(appForPdf, fs);

        _db.Certificates.Add(new Certificate
        {
            ApplicationId = app.Id,
            CertificateFile = relative
        });
    }
}
