using Microsoft.AspNetCore.Mvc;
using Prog7312PoePart1.Models;
using System.Diagnostics;

namespace Prog7312PoePart1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Main landing page
            return View();
        }

        public IActionResult Privacy()
        {
            // Privacy policy page
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Error page, uses ErrorViewModel
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
