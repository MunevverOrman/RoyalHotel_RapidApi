using Microsoft.AspNetCore.Mvc;

namespace RoyalHotel_RapidApi.ViewComponents
{
    public class _DefaultFooterComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
