using OptovayaBaza.Models;
using OptovayaBaza.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Util;

namespace OptovayaBaza.Controllers
{
    public class AdminController : Controller
    {
        private OptovayaBazaContext db;
        public AdminController()
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
        /// GET: /Admin/Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Admin/AllTransactions
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public ActionResult AllTransactions(DateTime? from, DateTime? to)
        {
            var query = db.Transactions.AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(t => t.DateTime >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(t => t.DateTime <= to.Value);
            }

            var result = query.OrderByDescending(t => t.DateTime).ToList();

            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            return View(result);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}