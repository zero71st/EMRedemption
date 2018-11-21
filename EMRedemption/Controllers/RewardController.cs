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
using NPOI.SS.UserModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Specialized;

namespace EMRedemption.Controllers
{
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RewardController> _logger;
        private readonly ITDesCryptoService _cryptoSerivce;
        private IHostingEnvironment _hostingEnvironment;

        public RewardController(ApplicationDbContext db,
                                ILogger<RewardController> logger,
                                ITDesCryptoService cryptoService,
                                IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _logger = logger;
            _cryptoSerivce = cryptoService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index(int RewardTypeId,string filterName, string keyword)
        {
            List<string> filters = new List<string>()
            {
                RewardStock.Avaliable,
                RewardStock.IsInUsed,
                RewardStock.All
            };

            var rewardTypes = _db.RewardTypes.Select(rw => new { rw.Id, rw.RewardName }).ToList();

            var lookupType = new SelectList(rewardTypes, "Id", "RewardName");

            if (RewardTypeId == 0)
                RewardTypeId = rewardTypes[0].Id;

            var rewards = _db.Rewards
                             .Where(rw=> rw.RewardTypeId == RewardTypeId)
                             .AsEnumerable();

            if (!String.IsNullOrEmpty(keyword))
                rewards = rewards.Where(c => c.Description.Contains(keyword));

            if (String.IsNullOrEmpty(filterName))
                filterName = RewardStock.Avaliable;

            if (filterName.Equals(RewardStock.Avaliable))
            {
                rewards = rewards.Where(r => r.RedemptionItemId == null || r.RedemptionItemId == 0);
            }

            if (filterName.Equals(RewardStock.IsInUsed))
            {
                rewards = rewards.Where(r => r.RedemptionItemId > 0);
            }

            var i = 0;
            var models = rewards
                        .OrderBy(rw => rw.RewardType)
                        .ThenBy(rw => rw.RewardCode)
                        .ThenBy(rw => rw.LotNo)
                        .ToList()
                        .Select(rw =>
                        {
                            i++;
                            return new RewardViewModel
                            {
                                LineNo = i,
                                Id = rw.Id,
                                RewardCode = rw.RewardCode,
                                RewardName = rw.RewardName,
                                RewardTypeName = rw.RewardTypeName,
                                SerialNo = rw.SerialNo,
                                Description = rw.Description,
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
            model.RewardTypes = lookupType;
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
        public ActionResult Create([Bind("RewardCode,SerialNo,Description,RewardType,ExpireDate,Quantity,LotNo")] RewardViewModel model)
        {

            var rewardType = _db.RewardTypes.SingleOrDefault(m => m.Code == model.RewardCode);

            if (rewardType == null)
            {
                ModelState.AddModelError("", "Can not found reward code: " + model.RewardCode);
                return View(model);
            }
            else
            {
                model.RewardName = rewardType.RewardName;
                model.RewardTypeName = rewardType.RewardTypeName;
            }

            try
            {
                if (ModelState.IsValid)
                {

                    Reward reward = new Reward(model.RewardCode,
                                               model.RewardName,
                                               _cryptoSerivce.Encrypt(model.SerialNo),
                                               model.Description,
                                               model.RewardTypeName,
                                               model.ExpireDate,
                                               model.Quantity,
                                               model.LotNo,
                                               User.Identity.Name);
                    reward.RewardTypeId = rewardType.Id;

                    _db.Add(reward);
                    _db.SaveChanges();
                    _logger.LogInformation("Create reward code {0} successful by {1} ", reward.RewardCode, User.Identity.Name);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Try to save reward by {0} with problem {1}", User.Identity.Name, ex.Message);
                ModelState.AddModelError("", "Can not save reward!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Try to save reward by {0} with problem {1}", User.Identity.Name, ex.Message);
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

        public ActionResult ImportRewards()
        {
            var items = _db.RewardTypes.Select(r => new { r.Id, Name = $"{r.Code}:{r.RewardName}" }).ToList();
            var model = new ImportRewardViewModel();
            model.LotDate = DateTime.Now;
            model.RewardTypes = new SelectList(items, "Id", "Name");
            model.RewardTypeId = items[0].Id;

            return View(model);
        }

        public ActionResult OnReadRewards()
        {
            var rewards = GetRewardsFromExcel(Request);

            StringBuilder sb = new StringBuilder();

            sb.Append("<table class='table'>");
            sb.Append("<tr>");
            sb.Append("<th>Lot No.</th>");
            sb.Append("<th>Reward Type</th>");
            sb.Append("<th>Reward Code</th>");
            sb.Append("<th>Reward Name</th>");
            sb.Append("<th>Serial No</th>");
            sb.Append("<th>Quantity</th>");
            sb.Append("<th>Expire Date</th>");
            sb.Append("</tr>");
                
            foreach (var reward in rewards)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{reward.LotNo}</td>");
                sb.Append($"<td>{reward.RewardTypeName}</td>");
                sb.Append($"<td>{reward.RewardCode}</td>");
                sb.Append($"<td>{reward.RewardName}</td>");
                sb.Append($"<td>{reward.SerialNo}</td>");
                sb.Append($"<td>{reward.Quantity}</td>");
                sb.Append($"<td>{reward.ExpireDate}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");

            return this.Content(sb.ToString());
        }

        private IEnumerable<Reward> GetRewardsFromExcel(HttpRequest request)
        {
            IFormCollection form = request.Form;
            IFormFile file = request.Form.Files[0];
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();

            int id = int.Parse(form["RewardId"]);
            string lot = form["LotDate"];
            string description = form["Description"];

            var type = _db.RewardTypes.FirstOrDefault(r => r.Id == id);

            if (type == null)
                throw new Exception("Reward Type can not be null!");

            var rewards = new List<Reward>();

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }

                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;

                    for (int j = 0; j < cellCount; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    }

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        Reward reward = new Reward();

                        reward.LotNo = lot;
                        reward.RewardCode = type.Code;
                        reward.RewardTypeName = type.RewardTypeName;
                        reward.RewardTypeId = type.Id;
                        reward.Quantity = 1;
                        reward.Description = description;
                        reward.AddDate = DateTime.Now;
                        reward.AddBy = User.Identity.Name;

                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                if (j == 0)
                                    reward.SerialNo = _cryptoSerivce.Encrypt(row.GetCell(j).ToString());
                                if (j == 1)
                                    reward.RewardName = row.GetCell(j).ToString();
                                if (j == 2)
                                    reward.ExpireDate = StringToDate(row.GetCell(j).ToString());
                            }
                        }
                        rewards.Add(reward);
                    }
                }
            }
            return rewards;
        }

        private DateTime StringToDate(string dateString)
        {
            return Convert.ToDateTime(dateString);
        }

        public ActionResult OnImportRewards()
        {
            var rewards = GetRewardsFromExcel(Request);
            try
            {
                //_db.Rewards.AddRange(rewards);
                foreach (var reward in rewards)
                {
                    _db.Rewards.Add(reward);
                    _db.SaveChanges();
                }

                _logger.LogInformation($"Import rewards by {User.Identity.Name} successful");

                return this.Content("<p class='alert alert-success'>Save successful</p>");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Can not import reward from excel by {User.Identity.Name}");
                return this.Content("<p class='alert alert-danger'>Save failed</p>");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,RewardCode,SerialNo,Description,RewardType,ExpireDate,Quantity,LotNo")]RewardViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            var rewardType = _db.RewardTypes.SingleOrDefault(m => m.Code == model.RewardCode);

            if (rewardType == null)
            {
                ModelState.AddModelError("", "Can not found reward code: " + model.RewardCode);
                return View(model);
            }
            else
            {
                model.RewardName = rewardType.RewardName;
                model.RewardTypeName = rewardType.RewardTypeName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var reward = _db.Rewards.Find(id);
                    reward.RewardCode = model.RewardCode;
                    reward.RewardName = model.RewardName;
                    reward.RewardTypeName = model.RewardTypeName;
                    reward.SerialNo = _cryptoSerivce.Encrypt(model.SerialNo);
                    reward.Description = model.Description;
                    reward.ExpireDate = model.ExpireDate;
                    reward.Quantity = model.Quantity;
                    reward.LotNo = model.LotNo.ToString("yyyy-MM-dd");
                    reward.RewardTypeId = rewardType.Id;

                    _db.SaveChanges();

                    _logger.LogInformation("Update reward code: {0} successful by {1}", reward.RewardCode, User.Identity.Name);
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