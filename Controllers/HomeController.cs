using Microsoft.AspNetCore.Mvc;

namespace RailwayReservationMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
