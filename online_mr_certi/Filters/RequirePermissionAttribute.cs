using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using online_mr_certi.Data;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;

namespace online_mr_certi.Filters;

/// <summary>
/// Enforces permission checks in backend (not UI-only).
/// Admin (Role string) is always allowed.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _permission;

    public RequirePermissionAttribute(string permission)
    {
        _permission = permission;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var session = context.HttpContext.Session;

        var roleString = session.GetString(SessionKeys.Role);
        if (roleString == AppRoles.Admin)
        {
            await next();
            return;
        }

        var userId = session.GetInt32(SessionKeys.UserId);
        if (userId is null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

        var hasPermission = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId && u.RoleId != null)
            .Join(db.RolePermissions.AsNoTracking(),
                u => u.RoleId!.Value,
                rp => rp.RoleId,
                (u, rp) => rp)
            .Join(db.Permissions.AsNoTracking(),
                rp => rp.PermissionId,
                p => p.Id,
                (rp, p) => p)
            .AnyAsync(p => p.Name == _permission);

        if (!hasPermission)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            return;
        }

        await next();
    }
}

