using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Params;
using TurretWebServiceCore.Data;
using TurretWebServiceCore.Models;
using TurretWebServiceCore.Tools;

namespace TurretWebServiceCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly TurretDBContext db;

        public UsersController(TurretDBContext dBContext)
        {
            db = dBContext;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/Users/getbyname/{name}
        [HttpGet("getbyname/{name?}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByName(string name)
        {
            List<User> users = GetUsersByNameOrContain(name, UsersSearchParam.ByName);
            if (users.Count == 0) return NotFound();
            return Ok(users);
        }

        // GET: api/Users/getifcontain/{substring}
        [HttpGet("getifcontain/{substring?}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetIfContain(string substring)
        {
            List<User> users = GetUsersByNameOrContain(substring, UsersSearchParam.IfContain);
            if (users.Count == 0) return NotFound();
            return Ok(users);
        }

        // GET: api/Users/gettop/{topCount}
        [HttpGet("gettop/{topCount?}/{sortParam?}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTop(int topCount, string sortParam = "s")
        {
            TopSortParam topSort;

            switch (sortParam)
            {
                case "s":
                    topSort = TopSortParam.SortByMaxScore;
                    break;
                case "l":
                    topSort = TopSortParam.SortByMaxLevel;
                    break;
                default:
                    topSort = TopSortParam.SortByMaxScore;
                    break;
            }

            List<User> users = GetTop(topCount, topSort);
            if (users.Count == 0) return NotFound();
            return Ok(users);
        }

        // POST: api/Users
        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult PostUser([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (db.Users.FirstOrDefault(u => u.Name == user.Name) != null)
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже существует.");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute(new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [Authorize(Roles ="administrator, user")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult PutUser(int id, [FromBody]User user)
        {
            //Пользователь в роли "user" может редактировать только самого себя и не может других пользователей.
            if (User.IsInRole("user") && !User.Identity.Name.Equals(user.Name))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }

        // DELETE: api/Users/5
        [Authorize(Roles = "administrator")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }

        private List<User> GetUsersByNameOrContain(string nameOrContain, UsersSearchParam searchParam)
        {
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            switch (searchParam)
            {
                case UsersSearchParam.ByName: return db.Users.ToList().FindAll((u) => u.Name.IndexOf(nameOrContain, comparison) == 0);
                case UsersSearchParam.IfContain: return db.Users.ToList().FindAll((u) => u.Name.IndexOf(nameOrContain, comparison) >= 0);
                default: return null;
            }
        }

        private List<User> GetTop(int topCount, TopSortParam sortParam = TopSortParam.SortByMaxScore)
        {
            List<User> users = db.Users.ToList();
            UserComparer comparer = null;

            switch (sortParam)
            {
                case TopSortParam.SortByMaxScore:
                    comparer = new UserScoreComparer();
                    break;

                case TopSortParam.SortByMaxLevel:
                    comparer = new UserLevelComparer();
                    break;
            }

            users.Sort(comparer);
            if (topCount > users.Count) topCount = users.Count;
            return users.GetRange(0, topCount);
        }
    }
}
