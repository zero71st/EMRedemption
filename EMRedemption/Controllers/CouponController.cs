using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EMRedemption.Models.CouponViewModels;
using EMRedemption.Data;
using EMRedemption.Entities;
using Microsoft.AspNetCore.Identity;
using EMRedemption.Models;

namespace EMRedemption.Controllers
{
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CouponController(
            ApplicationDbContext db,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _db = db;
            _signInManager = signInManager;
        }

        [ActionName("Index")]
        public ActionResult List()
        {
            var i = 0;
            var models = _db.Coupons.AsEnumerable()
                        .Select(c =>
                        {
                            i++;
                            return new CouponViewModel
                            {
                                LineNo = i,
                                Id = c.Id,
                                Code = c.Code,
                                ExpireDate = c.ExpireDate,
                                IsUsed = c.RedemptionId != null ? true : false,
                                AddBy = c.AddBy,
                                AddDate = c.AddDate,
                            };
                        }).ToList();

            return View(models);
        }

        public ActionResult Create()
        {
            return View();
        }

        // GET: Coupon/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Code,CouponPrice,ExpireDate")] CouponViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                
                Coupon coupon = new Coupon(model.Code, model.Price, model.ExpireDate,User.Identity.Name);

                _db.Add(coupon);
                _db.SaveChanges();

                return RedirectToAction(nameof(List));
            }
            catch
            {
                return View(model);
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