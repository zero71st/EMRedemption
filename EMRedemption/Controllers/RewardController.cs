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
using Microsoft.AspNetCore.Mvc.Rendering;

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
        //[Authorize]
        public ActionResult Index(string filterName,string keyword)
        {

            List<string> filters = new List<string>()
            {
                RewardStock.Avalible,
                RewardStock.IsInUsed,
                RewardStock.All
            };

            var rewards = _db.Rewards.AsEnumerable();

            if (!String.IsNullOrEmpty(keyword))
            {
                rewards = rewards.Where(c => c.Code.Contains(keyword) ||
                                             c.SerialNo.Contains(keyword) ||
                                             c.Description.Contains(keyword));
            }

            if (String.IsNullOrEmpty(filterName))
                filterName = RewardStock.Avalible;

            if (filterName.Equals(RewardStock.Avalible))
            {
                rewards = rewards.Where(r => r.RedemptionItemId == null || r.RedemptionItemId == 0);
            }

            if (filterName.Equals(RewardStock.IsInUsed))
            {
                rewards = rewards.Where(r => r.RedemptionItemId > 0);
            }

            var i = 0;
            var models = rewards.OrderBy(r=> r.RewardType)
                        .ThenBy(r=> r.Code)
                        .Select(r =>
                        {
                            i++;
                            return new RewardViewModel
                            {
                                LineNo = i,
                                Id = r.Id,
                                Code = r.Code,
                                SerialNo = r.SerialNo,
                                Description = r.Description,
                                RewardType = r.RewardType,
                                ExpireDate = r.ExpireDate,
                                Quantity = r.Quantity,
                                IsUsed = r.RedemptionItemId != null ? true : false,
                                AddBy = r.AddBy,
                                AddDate = r.AddDate,
                            };
                        })
                        .ToList();

            var model = new RewardListViewModel();
            model.Rewards = models;
            model.Filters = new SelectList(filters);
            model.FilterName = filterName;
            model.Keyword = keyword;

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new RewardViewModel();
            model.ExpireDate = DateTime.Now.Date;
            model.Quantity = 1;
            return View(model);
        }
        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Code,SerialNo,Description,RewardType,ExpireDate,Quantity")] RewardViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                Reward reward = new Reward(model.Code,model.SerialNo, model.Description,model.RewardType, model.ExpireDate,model.Quantity,User.Identity.Name);

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
            var reward = _db.Rewards.Find(id);
            if (reward == null)
                return NotFound();

            RewardViewModel model = new RewardViewModel(reward);
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,[Bind("Id,Code,SerialNo,Description,RewardType,ExpireDate,Quantity")]RewardViewModel model)
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
                    reward.Quantity = model.Quantity;

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