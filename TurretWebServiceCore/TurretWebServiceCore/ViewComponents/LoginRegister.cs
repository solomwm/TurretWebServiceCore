using Microsoft.AspNetCore.Mvc;

namespace TurretWebServiceCore.ViewComponents
{
    public class LoginRegister: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("_LoginRegister");
        }
    }
}