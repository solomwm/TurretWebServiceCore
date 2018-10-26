using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurretWebServiceCore.Models;

namespace TurretWebServiceCore.Controllers
{
    public class HomeController : Controller
    {
        //[Authorize(Roles = "administrator, user")]
        public IActionResult Index()
        {
            //string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            //return Content(User.Identity.Name + $"\nYour role is {role}");
            return View();
        }

        //[Authorize(Roles = "administrator")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
