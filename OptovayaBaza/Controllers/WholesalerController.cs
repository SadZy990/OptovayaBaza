using System;
using System.Linq;
using System.Web.Mvc;
using OptovayaBaza.Models;
using OptovayaBaza.Helpers;

namespace OptovayaBaza.Controllers
{
    public class WholesalerController : Controller
    {
        private OptovayaBazaContext db;
        public WholesalerController()
        {
            db = DbContextSingleton.GetInstance();
        }
        /// <summary>
        ///Проверка_авторизации_(оптовик)
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserId"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Account");
            }
            else if (Session["UserRole"]?.ToString() != "Wholesaler")
            {
                filterContext.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(filterContext);
        }


        /// <summary>
        ///ПРОСМОТР_ОСТАТКОВ_МАТЕРИАЛОВ
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var materials = db.Materials.ToList();
            return View(materials);
        }


        /// <summary>
        ///ОФОРМЛЕНИЕ_ОТГРУЗКИ_(GET)
        /// </summary>
        /// <returns></returns>
        public ActionResult Shipment()
        {
            ViewBag.Materials = new SelectList(db.Materials.Where(m => m.Stock > 0), "Id", "Name");
            return View();
        }

        /// <summary>
        /// ОФОРМЛЕНИЕ_ОТГРУЗКИ_(POST)
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="quantity"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Shipment(int materialId, decimal quantity, string comment)
        {
            var material = db.Materials.Find(materialId);
            int wholesalerId = (int)Session["UserId"];

            if (material == null)
            {
                ModelState.AddModelError("", "Материал не найден");
                ViewBag.Materials = new SelectList(db.Materials.Where(m => m.Stock > 0), "Id", "Name");
                return View();
            }

            if (material.Stock < quantity)
            {
                ModelState.AddModelError("", $"Недостаточно материала. Доступно: {material.Stock} {material.Unit}");
                ViewBag.Materials = new SelectList(db.Materials.Where(m => m.Stock > 0), "Id", "Name");
                return View();
            }

            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Количество должно быть больше 0");
                ViewBag.Materials = new SelectList(db.Materials.Where(m => m.Stock > 0), "Id", "Name");
                return View();
            }

            //СПИСАНИЕ ОСТАТКА
            material.Stock -= quantity;

            //СОЗДАНИЕ_ОПЕРАЦИИ_ОТГРУЗКИ
            var transaction = new Transaction
            {
                DateTime = DateTime.Now,
                Type = "Shipment",
                Quantity = quantity,
                Comment = comment ?? "Отгрузка материала",
                MaterialId = materialId,
                WholesalerId = wholesalerId,
                AdminId = null
            };

            db.Transactions.Add(transaction);
            db.SaveChanges();

            TempData["Success"] = $"Отгрузка {quantity} {material.Unit} выполнена успешно!";
            return RedirectToAction("Index");
        }


        /// <summary>
        /// ОФОРМЛЕНИЕ_ПРИНЯТИЯ_(ВОЗВРАТ)_(GET)
        /// </summary>
        /// <returns></returns>
        public ActionResult Receipt()
        {
            // Для возврата показываем все материалы (не только с остатком)
            ViewBag.Materials = new SelectList(db.Materials, "Id", "Name");
            return View();
        }

        /// <summary>
        /// ОФОРМЛЕНИЕ ПРИНЯТИЯ (ВОЗВРАТ) - POST
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="quantity"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Receipt(int materialId, decimal quantity, string comment)
        {
            var material = db.Materials.Find(materialId);
            int wholesalerId = (int)Session["UserId"];

            if (material == null)
            {
                ModelState.AddModelError("", "Материал не найден");
                ViewBag.Materials = new SelectList(db.Materials, "Id", "Name");
                return View();
            }

            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Количество должно быть больше 0");
                ViewBag.Materials = new SelectList(db.Materials, "Id", "Name");
                return View();
            }

            //УВЕЛИЧЕНИЕ_ОСТАТКА
            material.Stock += quantity;

            //СОЗДАНИЕ_ОПЕРАЦИИ_ПРИНЯТИЯ
            var transaction = new Transaction
            {
                DateTime = DateTime.Now,
                Type = "Receipt",
                Quantity = quantity,
                Comment = comment ?? "Возврат материала оптовиком",
                MaterialId = materialId,
                WholesalerId = wholesalerId,
                AdminId = null
            };

            db.Transactions.Add(transaction);
            db.SaveChanges();

            TempData["Success"] = $"Принятие (возврат) {quantity} {material.Unit} выполнено успешно!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// ИСТОРИЯ ОПЕРАЦИЙ (с фильтром по дате)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public ActionResult History(DateTime? from, DateTime? to)
        {
            int wholesalerId = (int)Session["UserId"];

            //Базовый_запрос
            var query = db.Transactions
                .Where(t => t.WholesalerId == wholesalerId);
            
            //По дате "с"
            if (from.HasValue)
            {
                DateTime fromDate = from.Value.Date; // начало дня
                query = query.Where(t => t.DateTime >= fromDate);
            }

            //По дате "по"
            if (to.HasValue)
            {
                DateTime toDate = to.Value.Date.AddDays(1); // конец дня (включительно)
                query = query.Where(t => t.DateTime < toDate);
            }

            //Сортировка_и_Выполнение
            var result = query.OrderByDescending(t => t.DateTime).ToList();

            //Значения для отображения
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