using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;

namespace online_mr_certi.Filters;

/// <summary>Restricts access to applicants (User role). Admins use the admin panel.</summary>
public sealed class RequireUserRoleAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var role = context.HttpContext.Session.GetString(SessionKeys.Role);
        if (string.IsNullOrEmpty(role) || role != AppRoles.User)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}
