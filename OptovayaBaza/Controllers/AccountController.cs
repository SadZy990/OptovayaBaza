using OptovayaBaza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OptovayaBaza.Helpers;

namespace OptovayaBaza.Controllers
{
    public class AccountController : Controller
    {
        private OptovayaBazaContext db;
        public AccountController()
        {
            db = DbContextSingleton.GetInstance();
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        /// <summary>
        /// GET: /Account/Login
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// POST: /Account/Login
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string login, string password)
        {
            // Поиск пользователя в базе данных
            var hashedPass = HashPassword(password);
            var user = db.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == hashedPass);

            if (user != null)
            {
                //Сессии 
                Session["UserId"] = user.Id;
                Session["UserRole"] = user.Role;
                Session["UserLogin"] = user.Login;

                // Перенаправление в зависимости от роли
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Wholesaler");
                }
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        /// <summary>
        /// GET: /Account/Logout
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}