using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;

namespace online_mr_certi.Controllers
{
    public class HomeController : Controller

    {
        // Action cusub oo Landing page-ka kuu furaya adigoon Dashboard lagugu celin
        public IActionResult Landing()
        {
            return View("Index");

        }
        public IActionResult Services()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32(SessionKeys.UserId) is int _)
            {
                var role = HttpContext.Session.GetString(SessionKeys.Role);
                return role == AppRoles.User
                    ? RedirectToAction("Index", "Dashboard")
                    : RedirectToAction("Index", "Admin");
            }
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
