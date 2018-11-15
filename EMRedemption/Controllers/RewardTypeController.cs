using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EMRedemption.Data;
using EMRedemption.Models.RewardTypeViewModels;
using Microsoft.Extensions.Logging;
using EMRedemption.Entities;
using Microsoft.AspNetCore.Authorization;

namespace EMRedemption.Controllers
{
    public class RewardTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RewardTypeController> _logger;

        public RewardTypeController(ApplicationDbContext db,
                                    ILogger<RewardTypeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: RewardType
        [Authorize]
        public async Task<IActionResult> Index(string keyward)
        {
            var rewardtypes = await _db.RewardTypes.ToListAsync();

            var model = new RewardTypeListViewModel();
            model.RewardTypes = rewardtypes.Select(r => new RewardTypeViewModel { Id = r.Id, RewardCode = r.Code, RewardName = r.RewardName, RewardTypeName = r.RewardTypeName }).ToList();

            return View(model);
        }

        // GET: RewardType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _db.RewardTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: RewardType/Create
        public IActionResult Create()
        {
            var model = new RewardTypeViewModel();
            model.RewardCode = GetNextCode();

            return View(model);
        }

        private string GetNextCode()
        {
            var rewardType = _db.RewardTypes.OrderByDescending(rt => rt.Code).FirstOrDefault();

            if (rewardType == null)
                return "00001";
            else
                return (int.Parse(rewardType.Code) + 1).ToString("00000");
        }

        // POST: RewardType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RewardCode,RewardName,RewardTypeName")] RewardTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.Add(new RewardType { Code = model.RewardCode, RewardName = model.RewardName, RewardTypeName = model.RewardTypeName });
                    await _db.SaveChangesAsync();

                    _logger.LogInformation("Reward Type Code:{0} was save by {1} successful",model.RewardCode,User.Identity.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Can not save new reward type by {0}",User.Identity.Name);
                    ModelState.AddModelError("", "Can not save!");
                }
            }

            return View(model);
        }

        // GET: RewardType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var rewardType = await _db.RewardTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (rewardType == null)
            {
                return NotFound();
            }

            var model = new RewardTypeViewModel { RewardCode = rewardType.Code, RewardName = rewardType.RewardName, RewardTypeName = rewardType.RewardTypeName };

            return View(model);
        }

        // POST: RewardType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RewardCode,RewardName,RewardTypeName")] RewardTypeViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var rewardType = _db.RewardTypes.Find(model.Id);
                    rewardType.Code = model.RewardCode;
                    rewardType.RewardName = model.RewardName;
                    rewardType.RewardTypeName = model.RewardTypeName;

                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Update reward type by {0} successful", User.Identity.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Can not update reward Type");
                    ModelState.AddModelError("", "Can not Save!");
                }
            }
            return View(model);
        }

        // GET: RewardType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rewardTypeViewModel = await _db.RewardTypes
                .SingleOrDefaultAsync(m => m.Id == id);
            if (rewardTypeViewModel == null)
            {
                return NotFound();
            }

            return View(rewardTypeViewModel);
        }

        // POST: RewardType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rewardTypeViewModel = await _db.RewardTypes.SingleOrDefaultAsync(m => m.Id == id);
            _db.RewardTypes.Remove(rewardTypeViewModel);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RewardTypeViewModelExists(int id)
        {
            return _db.RewardTypes.Any(e => e.Id == id);
        }
    }
}
