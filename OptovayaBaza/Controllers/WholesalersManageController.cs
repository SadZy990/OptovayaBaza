using OptovayaBaza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OptovayaBaza.Helpers;

namespace OptovayaBaza.Controllers
{
    public class WholesalersManageController : Controller
    {
        private OptovayaBazaContext db;
        public WholesalersManageController()
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
        /// Проверка авторизации (Admin)
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null || Session["UserRole"]?.ToString() != "Admin")
            {
                filterContext.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// GET: /WholesalersManage/Index (список оптовиков)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var wholesalers = db.Wholesalers.ToList();
            return View(wholesalers);
        }

        /// <summary>
        /// GET: /WholesalersManage/Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: /WholesalersManage/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WholesalerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка уникальности логина
                if (db.Users.Any(u => u.Login == model.Login))
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                    return View(model);
                }

                //add new USER
                var user = new User
                {
                    Login = model.Login,
                    PasswordHash = HashPassword(model.Password),
                    Role = "Wholesaler",
                    OrganizationName = model.OrganizationName,
                    ContactPerson = model.ContactPerson,
                    Phone = model.Phone
                };

                db.Users.Add(user);
                db.SaveChanges();

                //Add new Wholesalers
                var wholesaler = new Wholesaler
                {
                    OrganizationName = model.OrganizationName,
                    INN = model.INN,
                    Phone = model.Phone,
                    LegalAddress = model.LegalAddress,
                    UserId = user.Id
                };

                db.Wholesalers.Add(wholesaler);
                db.SaveChanges();

                TempData["Success"] = $"Оптовик '{model.OrganizationName}' успешно добавлен!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        /// <summary>
        /// GET: /WholesalersManage/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var wholesaler = db.Wholesalers.Find(id);
            if (wholesaler == null)
            {
                return HttpNotFound();
            }

            var user = db.Users.Find(wholesaler.UserId);
            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new WholesalerViewModel
            {
                Id = wholesaler.Id,
                Login = user.Login,
                OrganizationName = wholesaler.OrganizationName,
                INN = wholesaler.INN,
                Phone = wholesaler.Phone,
                LegalAddress = wholesaler.LegalAddress,
                ContactPerson = user.ContactPerson
            };

            return View(model);
        }

        /// <summary>
        /// POST: /WholesalersManage/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, WholesalerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var wholesaler = db.Wholesalers.Find(id);
                if (wholesaler == null)
                {
                    return HttpNotFound();
                }

                var user = db.Users.Find(wholesaler.UserId);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Проверка login/pass
                if (user.Login != model.Login && db.Users.Any(u => u.Login == model.Login))
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                    return View(model);
                }

                //Update USER
                user.Login = model.Login;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.PasswordHash = HashPassword(model.Password); 
                }
                user.OrganizationName = model.OrganizationName;
                user.ContactPerson = model.ContactPerson;
                user.Phone = model.Phone;

                //Update USER Wholesalers
                wholesaler.OrganizationName = model.OrganizationName;
                wholesaler.INN = model.INN;
                wholesaler.Phone = model.Phone;
                wholesaler.LegalAddress = model.LegalAddress;

                db.SaveChanges();

                TempData["Success"] = $"Оптовик '{model.OrganizationName}' успешно обновлён!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        /// <summary>
        /// GET: /WholesalersManage/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var wholesaler = db.Wholesalers.Find(id);
            if (wholesaler == null)
            {
                return HttpNotFound();
            }

            var user = db.Users.Find(wholesaler.UserId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Проверка, есть ли операции у оптовика
            var hasTransactions = db.Transactions.Any(t => t.WholesalerId == user.Id);
            if (hasTransactions)
            {
                TempData["Error"] = "Нельзя удалить оптовика, так как есть операции с ним!";
                return RedirectToAction("Index");
            }

            ViewBag.OrganizationName = wholesaler.OrganizationName;
            ViewBag.Login = user.Login;

            return View();
        }


        /// <summary>
        /// POST: /WholesalersManage/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var wholesaler = db.Wholesalers.Find(id);
            if (wholesaler != null)
            {
                var userId = wholesaler.UserId;

                //Del Wholesalers
                db.Wholesalers.Remove(wholesaler);

                //Del user
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    db.Users.Remove(user);
                }

                db.SaveChanges();
                TempData["Success"] = "Оптовик успешно удалён!";
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}