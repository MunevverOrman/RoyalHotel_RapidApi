using Microsoft.AspNetCore.Mvc;

namespace RoyalHotel_RapidApi.ViewComponents
{
    public class _DefaultScriptComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
