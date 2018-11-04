using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TurretWebServiceCore.Data;
using TurretWebServiceCore.Models;
using TurretWebServiceCore.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;
using Params;

namespace TurretWebServiceCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly TurretDBContext db;

        public AccountController(TurretDBContext dBContext)
        {
            db = dBContext;
            DatabaseInitialize(); // добавляем пользователя и роли в бд
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Name == model.Name && u.Password == PasswordService.GetPasswordHash(model.Password));
                if (user != null)
                {
                    await Authenticate(user); // Аутентификация
                    return RedirectToAction("Index", "Home");
                }
                else ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Name == model.Name);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user = new User { Name = model.Name, Password = PasswordService.GetPasswordHash(model.Password), MaxLevel = 0, MaxScore = 0 };
                    Role userRole = db.Roles.FirstOrDefault(r => r.Name == "user");
                    if (userRole != null) SetUserRole(ref user, ref userRole);
                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    
                    //аутентификация
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }
                else ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        private void DatabaseInitialize()
        {
            // если нет пользователей, добавляем их.
            if (!db.Users.Any())
            {
                db.Users.Add(new User { Name = "Винни-Пух", Password = PasswordService.GetPasswordHash("123456"), MaxLevel = 0, MaxScore = 0 });
                db.Users.Add(new User { Name = "Пятачок", Password = PasswordService.GetPasswordHash("123456"), MaxLevel = 0, MaxScore = 0 });
                db.Users.Add(new User { Name = "Кролик", Password = PasswordService.GetPasswordHash("123456"), MaxLevel = 0, MaxScore = 0 });
                db.SaveChanges();
            }

            if (!db.Roles.Any())
            {
                string adminRoleName = "administrator";
                string userRoleName = "user";
                string adminName = "admin";
                string adminPassword = PasswordService.GetPasswordHash("admin");

                // добавляем роли
                Role adminRole = new Role() { Name = adminRoleName };
                Role userRole = new Role() { Name = userRoleName };

                db.Roles.Add(adminRole);
                db.Roles.Add(userRole);

                // добавляем администратора
                var admin = new User() { Name = adminName, Password = adminPassword, Role = adminRole, RoleId = adminRole.Id };
                db.Users.Add(admin);
                adminRole.Users.Add(admin);

                //добавляем роли пользователям
                var users = db.Users;
                User user;
                foreach (var u in users)
                {
                    if (u.Role == null)
                    {
                        user = u;
                        SetUserRole(ref user, ref userRole);
                        db.Entry(user).State = EntityState.Modified;
                    }
                }
                //не забываем сохранить изменения в базе
                db.SaveChanges();
            }
        }
        
        //Устанавливает роль пользователю
        private void SetUserRole(ref User user, ref Role role)
        {
            user.Role = role;
            user.RoleId = role.Id;
            role.Users.Add(user);
        }
    }
}