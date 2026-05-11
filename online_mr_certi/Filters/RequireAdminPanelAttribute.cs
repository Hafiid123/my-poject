using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;

namespace online_mr_certi.Filters;

/// <summary>
/// Allows access to Admin panel for Admin + Staff, blocks normal Users.
/// </summary>
public sealed class RequireAdminPanelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.Session.GetInt32(SessionKeys.UserId);
        if (userId is null)
        {
            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
            return;
        }

        var role = context.HttpContext.Session.GetString(SessionKeys.Role);
        if (string.IsNullOrEmpty(role) || role == AppRoles.User)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}

