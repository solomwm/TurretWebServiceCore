using Microsoft.AspNetCore.Mvc;

namespace TurretWebServiceCore.ViewComponents
{
    public class WelcomeLogout: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_WelcomeLogout", User.Identity.Name);
        }
    }
}
