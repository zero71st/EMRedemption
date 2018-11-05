using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMRedemption.Models.RedemptionViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using EMRedemption.Models.Jsons;
using EMRedemption.Data;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using EMRedemption.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMRedemption.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMRedemption.Controllers
{
    public class RedemptionController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly ApplicationDbContext _db;

        public RedemptionController(ApplicationDbContext db,
                                    IConfiguration configuraton)
        {
            _db = db;

            Configuration = configuraton;
        }

        [Authorize]
        public IActionResult Index(string filterName,string keyword)
        {
            List<string> filters = new List<string>()
            {
                RedemptionProcess.All,
                RedemptionProcess.Unprocess,
                RedemptionProcess.Processed,
                RedemptionProcess.SendMailSuccess,
                RedemptionProcess.UnsendEmailSuccess,
            };

            IEnumerable<Redemption> redemptions = _db.Redemptions
                                                  .Include(r => r.RedemptionItems)
                                                  .OrderBy(r=> r.RedeemDateTime)
                                                  .AsEnumerable();
           
            if(!String.IsNullOrEmpty(keyword))
            {
                redemptions = redemptions
                             .Where(r => r.RetailerName.Contains(keyword));
            }

            if (String.IsNullOrEmpty(filterName))
                filterName = RedemptionProcess.All;

            if (filterName.Equals(RedemptionProcess.Unprocess))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.Unprocess);

            if (filterName.Equals(RedemptionProcess.Processed))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.Processed);

            if (filterName.Equals(RedemptionProcess.SendMailSuccess))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.SendEmailSuccess);

            if (filterName.Equals(RedemptionProcess.UnsendEmailSuccess))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.UnemailSuccess);

            redemptions = redemptions.ToList();

            int i = 0;
 
            var redemptionView = redemptions.Select(r => { i++; return new RedemptionViewModel(i, r); });

            var model = new RedemptionListViewModel();
            model.Redemptions = redemptionView.ToList();
            model.Filter = new SelectList(filters);
            model.FilterName = filterName;
            model.Keyword = keyword;

            return View(model);
        }

        [Authorize]
        public IActionResult ProcessRewardList(string keyward)
        {
           
            IEnumerable<Redemption> redemptions = _db.Redemptions
                                                  .Where(r=> r.Status == RedemptionStatus.Unprocess)
                                                  .Include(r => r.RedemptionItems).AsEnumerable();

            if (!String.IsNullOrEmpty(keyward))
            {
                redemptions = redemptions
                             .Where(r => r.RetailerName.Contains(keyward));
            }

            redemptions = redemptions.ToList();

            int i = 0;

            var redemptionView = redemptions.Select(r => { i++; return new RedemptionViewModel(i, r); });

            var model = new RedemptionListViewModel();
            model.Redemptions = redemptionView.ToList();
            model.Keyword = keyward;

            return View(model);
        }

        [Authorize]
        public IActionResult SendEmailList(string filterName,string keyward)
        {
            IEnumerable<Redemption> redemptions = _db.Redemptions
                                                  .Include(r => r.RedemptionItems).AsEnumerable();

            if (!String.IsNullOrEmpty(keyward))
            {
                redemptions = redemptions
                             .Where(r => r.RetailerName.Contains(keyward));
            }

            if (filterName.Equals(RedemptionProcess.Unprocess))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.Unprocess);

            if (filterName.Equals(RedemptionProcess.Processed))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.Processed);

            if (filterName.Equals(RedemptionProcess.SendMailSuccess))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.SendEmailSuccess);

            redemptions = redemptions.ToList();

            int i = 0;

            var redemptionView = redemptions.Select(r => { i++; return new RedemptionViewModel(i, r); });

            var model = new RedemptionListViewModel();
            model.Redemptions = redemptionView.ToList();
            model.FilterName = filterName;
            model.Keyword = keyward;

            return View(model);
        }

        [Authorize]
        public IActionResult SendEmail()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult SendEmail(int id)
        {
            try
            {
                var redemptionsToSendEmail = _db.Redemptions.Where(r => r.Status == RedemptionStatus.Processed).ToList();
                if (redemptionsToSendEmail.Count > 0)
                {
                    redemptionsToSendEmail.ForEach(r => r.SetAsSendEmailSuccess());

                    _db.UpdateRange(redemptionsToSendEmail);
                    _db.SaveChanges();

                    return RedirectToAction(nameof(SendEmailList),"filterName="+RedemptionProcess.Processed);
                }
            }
            catch (Exception)
            {
                return View();
            }

            return View();
        }

        [Authorize]
        public IActionResult ChangeStatus(int id)
        {
            var redemption = _db.Redemptions.Find(id);

            var model = new RedemptionViewModel(redemption);

            return View(model);
        }
        

        [HttpGet]
        [Authorize]
        [Route("/Redemption/ConfirmToStore", Name = "confirmToStore")]
        public IActionResult ConfirmToStore(string redeemDate, int quantity,bool isSaveToDatabase)
        {
            var model = new ConfirmToStoreViewModel(redeemDate, quantity,isSaveToDatabase);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmToStore([Bind("RedeemDate")]ConfirmToStoreViewModel model)
        {
            try
            {
                var redemptions = await GetRedemptionsAsync(model.RedeemDate);
                _db.Redemptions.AddRange(redemptions);
                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/Redemption/ProcessRewards", Name = "processRewards")]
        public IActionResult ProcessRewards()
        {
            var itemGroup =     _db.RedemptionItems.Include(i=> i.Redemption)
                                 .Where(r => r.Redemption.Status == RedemptionStatus.Unprocess)
                                 .GroupBy(g=> g.RewardCode)
                                 .Select(g=> new
                                 {
                                     Code = g.FirstOrDefault().RewardCode,
                                     Name = g.FirstOrDefault().RewardName,
                                     Quantity = g.Sum(i=> i.Quantity),
                                 })
                                 .ToList();

            var model = new ProcessRewardListViewModel();

            var items = new List<ProcessRewardViewModel>();

            foreach (var g in itemGroup)
            {
                var item = new ProcessRewardViewModel();
                item.RewardCode = g.Code;
                item.RewardName = g.Name;
                item.Quantity = g.Quantity;

                item.Available = _db.Rewards.Where(rw => rw.RedemptionItemId == null)
                                            .Where(rw=> rw.Code.Equals(item.RewardCode))
                                            .Sum(rw => rw.Quantity);

                item.Balance = item.Available - item.Quantity;

                items.Add(item);
            }

            model.RedemptionTotal = _db.Redemptions.Where(r => r.Status == RedemptionStatus.Unprocess).Count();
            model.ProcessRewards.AddRange(items.OrderBy(i=> i.RewardCode));
            
            return View(model);
        }

        private List<Reward> GetAvailableRewards(string rewardCode)
        {
            return _db.Rewards
                    .Where(rw => rw.Code == rewardCode)
                    .Where(rw => rw.RedemptionItemId == null)
                    .OrderBy(rw => rw.LotNo)
                    .ToList();
        }

        [HttpPost]
        [Authorize]
        public IActionResult ProcessRewards(string dummy)
        {
            try
            {
                var redemptions = _db.Redemptions
                                     .Include(r=> r.RedemptionItems)
                                     .Where(r => r.Status == RedemptionStatus.Unprocess)
                                     .ToList();

                foreach (var redemption in redemptions)
                {
                    // Process only transaction that reward enough
                    if (!IsRewardEnough(redemption))
                        continue;

                    foreach (var item in redemption.RedemptionItems)
                    {
                        var rewards = GetAvailableRewards(item.RewardCode);

                        // Reward quatity must >= item.Quantity
                        if(rewards.Sum(rw=> rw.Quantity) >= item.Quantity)
                        {
                            for (int i = 1; i <= item.Quantity; i++)
                            {
                                foreach (var reward in rewards)
                                {
                                    reward.RedemptionItemId = item.Id;
                                    reward.Quantity = reward.Quantity-1;
                                }
                            }

                            _db.UpdateRange(rewards);
                        }
                    }

                    redemption.SetAsProcessed();
                    _db.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }
        }

        private bool IsRewardEnough(Redemption redemption)
        {
            foreach (var item in redemption.RedemptionItems)
            {
                if (item.Quantity > GetAvailableRewards(item.RewardCode).Sum(rw=> rw.Quantity))
                    return false;
            }

            return true;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Retrieve(string redeemDate)
        {
            ViewBag.RedeemDate = DateTime.Now.Date.ToString("yyyy-MM-dd");  

            if (String.IsNullOrEmpty(redeemDate))
                redeemDate = ViewBag.RedeemDate;
            else
                ViewBag.RedeemDate = redeemDate;

            var redemptions = await GetRedemptionsAsync(redeemDate);

            ViewBag.Quantity = redemptions.Count.ToString();

            string[] transIds = redemptions.Select(r => r.TransactionID).ToArray();

            var redempts = _db.Redemptions
                              .Where(r => transIds.Contains(r.TransactionID))
                              .Select(r => r.Id)
                              .ToList();

            ViewBag.IsSaveToDatabase = redempts.Count > 0;

            int j = 0;
            var models = redemptions.Select(r => { j++; return new RedemptionViewModel(j, r); }).ToList<RedemptionViewModel>();

            return View(models);
        }

        private async Task<List<Redemption>> GetRedemptionsAsync(string redeemDate)
        {
            var redemptions = new List<Redemption>();
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://test.thmobilloyaltyclub.com/api/redeem_voucher_list");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("accessKey", "thai$2R@88"),
                new KeyValuePair<string, string>("startDate", redeemDate+" 12:00 AM"),
                new KeyValuePair<string,string>("endDate",redeemDate+" 11:59 PM")
            });

            var resp = await client.PostAsync("", content);

            var jsonString = await resp.Content.ReadAsStringAsync();

            var jsons = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

            if (jsons.redeemDetails == null)
                return redemptions;

            foreach (var master in jsons.redeemDetails)
            {
                var redemption = new Redemption();
                redemption.TransactionID = master.TransactionID;
                redemption.RetailerName = master.retailerName;
                redemption.RetailerStoreName = master.retailerStoreName;
                redemption.RetailerEmailAddress = master.retailerEmailAddress;
                redemption.RetailerPhoneNumber = master.retailerPhoneNumber;
                redemption.RedeemDateTime = master.RedeemDateTime;
                redemption.FetchBy = User.Identity.Name;
                redemption.FetchDateTime = DateTime.Now;
                redemption.Status = RedemptionStatus.Unprocess;
                redemption.RedemptionItems.AddRange(master.productDetails.Select(i => new RedemptionItem
                {
                    RewardCode = i.productCode,
                    RewardName = i.productName,
                    Points     = i.points,
                    Quantity   = i.quantity,
                }));
        
                redemptions.Add(redemption);
            }

            return redemptions;
        }

        public async Task<IActionResult> CallApi(string startDate, string endDate)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://test.thmobilloyaltyclub.com/api/redeem_voucher_list");

            FormUrlEncodedContent content = null;

            if (!String.IsNullOrEmpty(startDate))
            {
                content = new FormUrlEncodedContent(new[]
                 {
                    new KeyValuePair<string, string>("accessKey", "thai$2R@88"),
                    new KeyValuePair<string, string>("startDate", startDate),
                    new KeyValuePair<string,string>("endDate",endDate)
                 });
            }
            else
            {
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("accessKey", "thai$2R@88"),
                });
            }

            var resp = await client.PostAsync("", content);

            var jsonString = await resp.Content.ReadAsStringAsync();

            var objects = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

            var models = new List<RedemptionViewModel>();
            int k = 0;
            foreach (var master in objects.redeemDetails)
            {

                var redemption = new RedemptionViewModel();
                k++;
                redemption.LineNo = k;
                redemption.TransactionID = master.TransactionID;
                redemption.RetailerName = master.retailerName;
                redemption.RetailerStoreName = master.retailerStoreName;
                redemption.RetailerEmailAddress = master.retailerEmailAddress;
                redemption.RetailerPhoneNumber = master.retailerPhoneNumber;
                redemption.RedeemDateTime = master.RedeemDateTime;

                var redemptionItems = new List<RedemptionItemViewModel>();

                int j = 0;
                foreach (var detail in master.productDetails)
                {
                    j++;
                    var item = new RedemptionItemViewModel();
                    item.LineNo = j;
                    item.RewardCode = detail.productCode;
                    item.RewardName = detail.productName;
                    item.Points = detail.points;
                    item.Quantity = detail.quantity;
                    redemptionItems.Add(item);
                }

                redemption.RedemptionItems.AddRange(redemptionItems);

                models.Add(redemption);
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (MySqlConnection conn = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
                    {
                        foreach (var model in models)
                        {
                            conn.Open();

                            string sql = "INSERT INTO Redemptions(TransactionID,RetailerName,RetailerStoreName,RetailerEmailAddress,RetailerPhoneNumber,RedeemDateTime,FetchDateTime)" +
                                                         "VALUES (@transactionID,@retialerName,@retialerStoreName,@retailerEmailAddress,@retailerPhoneNumber,@redeemDateTime,@fetchDateTime)";

                            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@transactionID", model.TransactionID);
                                cmd.Parameters.AddWithValue("@retialerName", model.RetailerName);
                                cmd.Parameters.AddWithValue("@retialerStoreName", model.RetailerStoreName);
                                cmd.Parameters.AddWithValue("@retailerEmailAddress", model.RetailerEmailAddress);
                                cmd.Parameters.AddWithValue("@retailerPhoneNumber", model.RetailerPhoneNumber);
                                cmd.Parameters.AddWithValue("@redeemDateTime", model.RedeemDateTime.ToString("yyyy-MM-dd H:mm:ss"));
                                cmd.Parameters.AddWithValue("@fetchDateTime", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));

                                cmd.ExecuteNonQuery();

                                foreach (var item in model.RedemptionItems)
                                {
                                    sql = "INSERT INTO RedemptionItems(RedemptionId,RewardCode,RewardName,Points,Quantity) VALUES (@redemptionID,@rewardCode,@rewardName,@points,@quantity)";
                                    using (MySqlCommand cmd2 = new MySqlCommand(sql, conn))
                                    {
                                        cmd2.Parameters.AddWithValue("@redemptionID", cmd.LastInsertedId);
                                        cmd2.Parameters.AddWithValue("@rewardCode", item.RewardCode);
                                        cmd2.Parameters.AddWithValue("@rewardName", item.RewardName);
                                        cmd2.Parameters.AddWithValue("@points", item.Points);
                                        cmd2.Parameters.AddWithValue("@quantity", item.Quantity);

                                        cmd2.ExecuteNonQuery();
                                    }
                                }
                            }
                            conn.Close();
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(models.OrderBy(m => m.RedeemDateTime).ThenBy(m => m.TransactionID));
        }
    }
}
