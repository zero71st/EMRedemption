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
using EMRedemption.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EMRedemption.Controllers
{
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RewardController> _logger;
        private readonly ITDesCryptoService _cryptoSerivce;

        public RewardController(ApplicationDbContext db,
                                ILogger<RewardController> logger,
                                ITDesCryptoService cryptoService)
        {
            _db = db;
            _logger = logger;
            _cryptoSerivce = cryptoService;
        }

        [HttpGet]
        [Authorize]
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
                rewards = rewards.Where(c => c.Description.Contains(keyword));

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
            var models = rewards
                        .OrderBy(rw=> rw.LotNo)
                        .OrderBy(rw=> rw.RewardType)
                        .ThenBy(rw=> rw.Code)
                        .ToList()
                        .Select(rw =>
                        {
                            i++;
                            return new RewardViewModel
                            {
                                LineNo = i,
                                Id = rw.Id,
                                Code = rw.Code,
                                SerialNo = rw.SerialNo,
                                Description = rw.Description,
                                RewardType = rw.RewardType,
                                ExpireDate = rw.ExpireDate,
                                Quantity = rw.Quantity,
                                LotNo = RewardViewModel.StringLotNoToDateLotNo(rw.LotNo),
                                IsUsed = rw.RedemptionItemId != null ? true : false,
                                AddBy = rw.AddBy,
                                AddDate = rw.AddDate,
                            };
                        }).ToList();

            var model = new RewardListViewModel();
            model.Rewards = models;
            model.Filters = new SelectList(filters);
            model.FilterName = filterName;
            model.Keyword = keyword;

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var model = new RewardViewModel();
            model.ExpireDate = DateTime.Now.Date;
            model.LotNo = DateTime.Now.Date;
            model.Quantity = 1;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Code,SerialNo,Description,RewardType,ExpireDate,Quantity,LotNo")] RewardViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Reward reward = new Reward(model.Code,
                                                       _cryptoSerivce.Encrypt(model.SerialNo),
                                                       model.Description,
                                                       model.RewardType,
                                                       model.ExpireDate,
                                                       model.Quantity,
                                                       model.LotNo,
                                                       User.Identity.Name);

                    _db.Add(reward);
                    _db.SaveChanges();
                    _logger.LogInformation("Create reward code {0} successful by {1} ",reward.Code,User.Identity.Name);
                    return RedirectToAction(nameof(Index)); 
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Try to save reward by {0} with problem {1}", User.Identity.Name,ex.Message);
                ModelState.AddModelError("", "Can not save reward!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Try to save reward by {0} with problem {1}", User.Identity.Name,ex.Message);
                ModelState.AddModelError("", "Can not save reward!");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var reward = _db.Rewards.Find(id);
            if (reward == null)
                return NotFound();

            RewardViewModel model = new RewardViewModel(reward);
            model.SerialNo = _cryptoSerivce.Decrypt(reward.SerialNo);
            
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,[Bind("Id,Code,SerialNo,Description,RewardType,ExpireDate,Quantity,LotNo")]RewardViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var reward = _db.Rewards.Find(id);
                    reward.Code = model.Code;
                    reward.SerialNo = _cryptoSerivce.Encrypt(model.SerialNo);
                    reward.Description = model.Description;
                    reward.RewardType = model.RewardType;
                    reward.ExpireDate = model.ExpireDate;
                    reward.Quantity = model.Quantity;
                    reward.LotNo = model.LotNo.ToString("yyyy-MM-dd");

                    _db.SaveChanges();

                    _logger.LogInformation("Update reward code: {0} successful by {1}",reward.Code, User.Identity.Name);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    _logger.LogError("Can not update reward! by {0} with problem {1}", User.Identity.Name, ex.Message);
                    ModelState.AddModelError("", "Can not update reward!");
                } 
            }

            return View(model);
        }
    }
}