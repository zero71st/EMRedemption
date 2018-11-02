using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EMRedemption.Models.RewardViewModels;
using EMRedemption.Data;
using EMRedemption.Entities;
using Microsoft.AspNetCore.Identity;
using EMRedemption.Models;
using Microsoft.AspNetCore.Authorization;

namespace EMRedemption.Controllers
{
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RewardController(
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
            var rewards = _db.Rewards.AsEnumerable();

            if (!String.IsNullOrEmpty(search))
            {
                rewards = rewards.Where(c => c.Code.Contains(search));
            }

            var i = 0;
            var models = rewards.Select(c =>
                        {
                            i++;
                            return new RewardViewModel
                            {
                                LineNo = i,
                                Id = c.Id,
                                Code = c.Code,
                                SerialNo = c.SerialNo,
                                Description = c.Description,
                                RewardType = c.RewardType,
                                ExpireDate = c.ExpireDate,
                                IsUsed = c.RedemptionItemId != null ? true : false,
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
        public ActionResult Create([Bind("Code,SerialNo,Description,RewardType,ExpireDate")] RewardViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                Reward reward = new Reward(model.Code,model.SerialNo, model.Description,model.RewardType, model.ExpireDate,User.Identity.Name);

                _db.Add(reward);
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
            var coupon = _db.Rewards.Find(id);
            if (coupon == null)
                return NotFound();

            RewardViewModel model = new RewardViewModel(coupon);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,[Bind("Id,Code,SerialNo,Description,RewardType,ExpireDate")]RewardViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var reward = _db.Rewards.Find(id);
                if (reward != null)
                {
                    reward.Code = model.Code;
                    reward.SerialNo = model.SerialNo;
                    reward.Description = model.Description;
                    reward.RewardType = model.RewardType;
                    reward.ExpireDate = model.ExpireDate;

                    _db.Update(reward);
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