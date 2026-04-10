using Microsoft.AspNetCore.Mvc;

namespace RoyalHotel_RapidApi.ViewComponents
{
    public class _DefaultLastBlogComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}