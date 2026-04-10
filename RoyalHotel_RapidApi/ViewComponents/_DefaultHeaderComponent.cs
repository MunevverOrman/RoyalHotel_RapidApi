using Microsoft.AspNetCore.Mvc;

namespace RoyalHotel_RapidApi.ViewComponents
{
    public class _DefaultHeaderComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
