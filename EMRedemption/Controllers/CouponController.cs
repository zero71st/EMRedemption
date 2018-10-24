using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EMRedemption.Models.CouponViewModels;
using EMRedemption.Data;

namespace EMRedemption.Controllers
{
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        [ActionName("Index")]
        public ActionResult List()
        {
            List<CouponViewModel> models = new List<CouponViewModel>()
            {
                new CouponViewModel {LineNo=1, Code = "123456", Price = 300, AddBy = "Kasem", AddDate = DateTime.Now, IsUsed = false},
                new CouponViewModel {LineNo=2, Code = "123456", Price = 300, AddBy = "Kasem", AddDate = DateTime.Now, IsUsed = false},
                new CouponViewModel {LineNo=3, Code = "123456", Price = 300, AddBy = "Kasem", AddDate = DateTime.Now, IsUsed = false},
                new CouponViewModel {LineNo=4, Code = "123456", Price = 300, AddBy = "Kasem", AddDate = DateTime.Now, IsUsed = false},
            };

            //var models = 

            return View(models);
        }

        // GET: Coupon/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Coupon/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(List));
            }
            catch
            {
                return View();
            }
        }

        // GET: Coupon/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Coupon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(List));
            }
            catch
            {
                return View();
            }
        }

        // GET: Coupon/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Coupon/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(List));
            }
            catch
            {
                return View();
            }
        }
    }
}