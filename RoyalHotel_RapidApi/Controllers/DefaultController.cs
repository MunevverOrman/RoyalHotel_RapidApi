using Microsoft.AspNetCore.Mvc;

namespace RoyalHotel_RapidApi.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
