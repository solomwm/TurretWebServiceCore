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
        [HttpGet]
        [Route("getbyname/{name?}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByName(string name)
        {
            var users = GetUsersByNameOrContain(name, UsersSearchParam.ByName);
            if (users.Count == 0) return NotFound();
            return Ok(users);
        }

        // GET: api/Users/getifcontain/{substring}
        [HttpGet]
        [Route("getifcontain/{substring?}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetIfContain(string substring)
        {
            var users = GetUsersByNameOrContain(substring, UsersSearchParam.IfContain);
            if (users.Count == 0) return NotFound();
            return Ok(users);
        }

        // POST: api/Users
        //[Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult PostUser([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute(new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        //[Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult PutUser(int id, [FromBody]User user)
        {
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

        // DELETE: api/ApiWithActions/5
        //[Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    }
}
