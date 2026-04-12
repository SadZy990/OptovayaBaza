using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OptovayaBaza.Models;
using OptovayaBaza.Helpers;


namespace OptovayaBaza.Controllers
{
    public class MaterialsController : Controller
    {
        private OptovayaBazaContext db;
        public MaterialsController()
        {
            db = DbContextSingleton.GetInstance();
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
        /// СПИСОК МАТЕРИАЛОВ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var materials = db.Materials.ToList();
            return View(materials);
        }

        /// <summary>
        /// ДОБАВЛЕНИЕ МАТЕРИАЛА
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Material material)
        {
            if (ModelState.IsValid)
            {
                db.Materials.Add(material);
                db.SaveChanges();
                TempData["Success"] = "Материал успешно добавлен!";
                return RedirectToAction("Index");
            }
            return View(material);
        }

        /// <summary>
        /// РЕДАКТИРОВАНИЕ МАТЕРИАЛА
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Material material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Материал успешно обновлён!";
                return RedirectToAction("Index");
            }
            return View(material);
        }

        /// <summary>
        /// УДАЛЕНИЕ МАТЕРИАЛА
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }

            // Проверка, есть ли операции с этим материалом
            var hasTransactions = db.Transactions.Any(t => t.MaterialId == id);
            ViewBag.HasTransactions = hasTransactions;

            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var material = db.Materials.Find(id);

            if (material == null)
            {
                return HttpNotFound();
            }

            // Проверка, есть ли операции с этим материалом
            var hasTransactions = db.Transactions.Any(t => t.MaterialId == id);
            if (hasTransactions)
            {
                TempData["Error"] = "Нельзя удалить материал, так как есть операции с ним!";
                return RedirectToAction("Index");
            }

            db.Materials.Remove(material);
            db.SaveChanges();
            TempData["Success"] = "Материал успешно удалён!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}