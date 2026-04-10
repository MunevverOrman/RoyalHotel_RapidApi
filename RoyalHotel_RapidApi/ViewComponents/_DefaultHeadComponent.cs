using Microsoft.AspNetCore.Mvc;

namespace RoyalHotel_RapidApi.ViewComponents
{
    public class _DefaultHeadComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }

}
