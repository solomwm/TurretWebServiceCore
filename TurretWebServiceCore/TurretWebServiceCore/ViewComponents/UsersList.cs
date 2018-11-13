using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TurretWebServiceCore.Data;

namespace TurretWebServiceCore.ViewComponents
{
    public class UsersList: ViewComponent
    {
        private readonly TurretDBContext db;

        public UsersList(TurretDBContext dBContext)
        {
            db = dBContext;
        }

        public IViewComponentResult Invoke(int topCount)
        {
            IEnumerable<TurretWebServiceCore.Models.User> users = db.Users;
            users = users.OrderByDescending(u => u.MaxScore).Take(topCount);
            return View("_TopUsers", users);
        }
    }
}