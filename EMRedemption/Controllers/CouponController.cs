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
using Microsoft.AspNetCore.Authorization;

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

        
        [HttpGet]
        [Authorize]
        public ActionResult Index(string search)
        {
            var coupons = _db.Coupons.AsEnumerable();

            if (!String.IsNullOrEmpty(search))
            {
                coupons = coupons.Where(c => c.Code.Contains(search));
            }

            var i = 0;
            var models = coupons.Select(c =>
                        {
                            i++;
                            return new CouponViewModel
                            {
                                LineNo = i,
                                Id = c.Id,
                                Code = c.Code,
                                Price = c.Price,
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
        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Code,Price,ExpireDate")] CouponViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                
                Coupon coupon = new Coupon(model.Code, model.Price, model.ExpireDate,User.Identity.Name);

                _db.Add(coupon);
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }

        // GET: Coupon/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var coupon = _db.Coupons.Find(id);
            if (coupon == null)
                return NotFound();

            CouponViewModel model = new CouponViewModel(coupon);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,[Bind("Id,Code,Price,ExpireDate")]CouponViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var coupon = _db.Coupons.Find(id);
                if (coupon != null)
                {
                    coupon.Code = model.Code;
                    coupon.Price = model.Price;
                    coupon.ExpireDate = model.ExpireDate;

                    _db.Update(coupon);
                    _db.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
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

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}