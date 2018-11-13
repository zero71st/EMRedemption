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
using System.Net.Mail;
using System.Net;
using EMRedemption.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMRedemption.Controllers
{
    public class RedemptionController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _mailSender;

        public RedemptionController(ApplicationDbContext db,
                                    IConfiguration configuraton,
                                    IEmailSender sender)
        {
            _db = db;
            Configuration = configuraton;
            _mailSender = sender;
        }

        [Authorize]
        public IActionResult Index(string filterName,string keyword)
        {
            List<string> filters = new List<string>()
            {
                RedemptionProcess.All,
                RedemptionProcess.Unprocess,
                RedemptionProcess.Processed,
                RedemptionProcess.DeliveredSuccessful,
                RedemptionProcess.UndeliverSuccessful,
            };

            IEnumerable<Redemption> redemptions = _db.Redemptions
                                                  .Include(r => r.RedemptionItems)
                                                  .OrderBy(r=> r.RedeemDateTime)
                                                  .AsEnumerable();
           
            if(!String.IsNullOrEmpty(keyword))
            {
                redemptions = redemptions
                             .Where(r => r.TransactionID.Contains(keyword) ||
                                         r.RetailerName.Contains(keyword) ||
                                         r.RetailerStoreName.Contains(keyword) ||
                                         r.RetailerPhoneNumber.Contains(keyword) ||
                                         r.RetailerEmailAddress.Contains(keyword));
            }

            if (String.IsNullOrEmpty(filterName))
                filterName = RedemptionProcess.All;

            if (filterName.Equals(RedemptionProcess.Unprocess))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.Unprocess);

            if (filterName.Equals(RedemptionProcess.Processed))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.Processed);

            if (filterName.Equals(RedemptionProcess.DeliveredSuccessful))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.DeliveredSuccessful);

            if (filterName.Equals(RedemptionProcess.UndeliverSuccessful))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.UndeliverSuccessful);

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
        public IActionResult ProcessRewardList(string keyword)
        {

            IEnumerable<Redemption> redemptions = _db.Redemptions
                                                  .Include(r => r.RedemptionItems)
                                                  .Where(r => r.Status == RedemptionStatus.Unprocess)
                                                  .AsEnumerable();

            if (!String.IsNullOrEmpty(keyword))
            {
                redemptions = redemptions.Where(r => r.TransactionID.Contains(keyword) ||
                                                     r.RetailerName.Contains(keyword)  ||
                                                     r.RetailerStoreName.Contains(keyword) ||
                                                     r.RetailerPhoneNumber.Contains(keyword) ||
                                                     r.RetailerEmailAddress.Contains(keyword));
            }

            redemptions = redemptions.ToList();

            int i = 0;

            var redemptionView = redemptions.Select(r => { i++; return new RedemptionViewModel(i, r); });

            var model = new RedemptionListViewModel();
            model.Redemptions = redemptionView.ToList();
            model.Keyword = keyword;

            return View(model);
        }

        [Authorize]
        public IActionResult SendEmailList(string filterName,string keyword)
        {
            IEnumerable<Redemption> redemptions = _db.Redemptions
                                                  .Include(r => r.RedemptionItems).AsEnumerable();

            if (!String.IsNullOrEmpty(keyword))
            {
                               redemptions = _db.Redemptions
                                                .Include(r => r.RedemptionItems)
                                                .Where(r => r.TransactionID.Contains(keyword) ||
                                                            r.RetailerName.Contains(keyword) ||
                                                            r.RetailerStoreName.Contains(keyword) ||
                                                            r.RetailerPhoneNumber.Contains(keyword) ||
                                                            r.RetailerEmailAddress.Contains(keyword)).AsEnumerable();
            }

            if (filterName.Equals(RedemptionProcess.Processed))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.Processed);

            if (filterName.Equals(RedemptionProcess.DeliveredSuccessful))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.DeliveredSuccessful);

            if (filterName.Equals(RedemptionProcess.UndeliverSuccessful))
                redemptions = redemptions.Where(r => r.Status == RedemptionStatus.UndeliverSuccessful);

            redemptions = redemptions.ToList();

            int i = 0;

            var redemptionView = redemptions.Select(r => { i++; return new RedemptionViewModel(i, r); });

            var model = new RedemptionListViewModel();
            model.Redemptions = redemptionView.ToList();
            model.FilterName = filterName;
            model.Keyword = keyword;

            return View(model);
        }

        [Authorize]
        public IActionResult SendEmail()
        {
            var model = new SendEmailViewModel();
            model.SendType = "Send Email";
            model.SendDateTime = DateTime.Now;
            model.TotalRedemptions = _db.Redemptions.Count(r => r.Status == RedemptionStatus.Processed);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SendEmail(int id)
        {
            try
            {
                var redemptions = _db.Redemptions
                                     .Where(r => r.Status == RedemptionStatus.Processed)
                                     .ToList();

                if (redemptions.Count > 0)
                {
                    try
                    {
                        foreach (var redemption in redemptions)
                        {
                            _mailSender.SendEmailAsync(redemption.RetailerEmailAddress,"Inform Code","Message");
                            redemption.SetAsDeliveredSuccessful();
                            _db.Update(redemption);
                            _db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return RedirectToAction(nameof(SendEmailList), "Redemption", new { @filterName = RedemptionProcess.Processed });
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

        [HttpPost]
        [Authorize]
        public IActionResult ChangeStatus(int id,RedemptionViewModel model)
        {
            try
            {
                var redemption = _db.Redemptions.Find(id);
                redemption.SetAsUndeliverSuccessful();

                _db.Update(redemption);
                _db.SaveChanges();

                return RedirectToAction(nameof(SendEmailList), new { @filterName = RedemptionProcess.DeliveredSuccessful });
            }
            catch (Exception)
            {
                return View();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult ResendEmail()
        {
            var model = new SendEmailViewModel();
            model.SendType = "Resend Email";
            model.SendDateTime = DateTime.Now;
            model.TotalRedemptions = _db.Redemptions.Count(r => r.Status == RedemptionStatus.UndeliverSuccessful);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult ResendEmail(int id)
        {
            var resendMails = _db.Redemptions.Where(c => c.Status == RedemptionStatus.UndeliverSuccessful).ToList();
            try
            {
                foreach (var resend in resendMails)
                {
                    _mailSender.SendEmailAsync(resend.RetailerEmailAddress, "Resend Redemption", "data");
                    resend.SetAsDeliveredSuccessful();
                    //_db.Update(resend);
                    _db.SaveChanges();

                    return RedirectToAction(nameof(SendEmailList), "Redemption", new {@filterName = RedemptionProcess.UndeliverSuccessful});
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmToStore([Bind("RedeemDate")]ConfirmToStoreViewModel model)
        {
            try
            {
                var redemptions = await GetRedemptionsAsync(model.RedeemDate);
                _db.Redemptions.AddRange(redemptions);
                _db.SaveChanges();

                return RedirectToAction(nameof(Retrieve));
            }
            catch (Exception)
            {
                return View();
            }
        }

        private IEnumerable<Reward> GetAvailableRewards()
        {
            return _db.Rewards
                      .Where(rw=> rw.Quantity > 0)
                      .OrderBy(rw => rw.LotNo)
                      .ThenBy(rw => rw.Code)
                      .ToList();
        }

        [HttpGet]
        [Authorize]
        [Route("/Redemption/ProcessRewards", Name = "processRewards")]
        public IActionResult ProcessRewards()
        {
            IList<ProcessRewardViewModel> models = new List<ProcessRewardViewModel>();

            var stockRewards = GetAvailableRewards();
            var redemptions =   _db.Redemptions
                                   .Include(r => r.RedemptionItems)
                                   .Where(r => r.Status == RedemptionStatus.Unprocess)
                                   .ToList();

            foreach (var redemption in redemptions)
            {
                bool skip = false;
                foreach (var item in redemption.RedemptionItems)
                {
                    int stock = stockRewards.Where(rw => rw.Code.Equals(item.RewardCode)).Sum(rw => rw.Quantity);

                    if (item.Quantity > stock)
                        skip = true;
                }

                if (skip) continue;

                foreach (var redemptionItem in redemption.RedemptionItems)
                {
                    var rewards = stockRewards.Where(rw => rw.Code.Equals(redemptionItem.RewardCode)).OrderBy(rw=> rw.LotNo);

                    // Reward quatity must >= item.Quantity
                    if (redemptionItem.Quantity <= rewards.Sum(rw => rw.Quantity))
                    {
                        foreach (var reward in rewards)
                        {
                            if (redemptionItem.Quantity == 0)
                                break;

                            if (redemptionItem.Quantity > reward.Quantity)
                            {
                                // 3=3-1;
                                // 2=2-1;
                                // 1=1-1
                                // 0

                                redemptionItem.Quantity = redemptionItem.Quantity - reward.Quantity;
                                reward.Quantity = 0;
                            }
                            else // RedemptionItem.Quantity <= reward.Quantity
                            {
                                // 5=5-3
                                // 3=3-3
                                // 0
                                reward.Quantity = reward.Quantity - redemptionItem.Quantity;
                                redemptionItem.Quantity = 0;
                            }

                            redemptionItem.Rewards.Add(reward);

                            models.Add(new ProcessRewardViewModel { RewardId = reward.Id, Quantity = reward.Quantity, RedemptionItemId = redemptionItem.Id, RedemptionId = redemption.Id, });
                        }
                    }
                }

                redemption.SetAsProcessed();
            }

            //Show summary for user
            ViewData["Redemptions"] = redemptions;

            return View(models);
        }

        [HttpPost]
        [Authorize]
        public IActionResult ProcessRewards(IList<ProcessRewardViewModel> models)
        {
            try
            {
                using (var trasaction = _db.Database.BeginTransaction())
                {
                    var redemptions = models.GroupBy(r => r.RedemptionId);

                    foreach (var redemption in redemptions)
                    {
                        var redemptionToUpdate = _db.Redemptions.Find(redemption.Key);

                        if (redemptionToUpdate != null)
                        {
                            foreach (var model in redemption)
                            {
                                var rewardToUpdate = _db.Rewards.Find(model.RewardId);

                                rewardToUpdate.RedemptionItemId = model.RedemptionItemId;
                                rewardToUpdate.Quantity = model.Quantity;

                                _db.Update(rewardToUpdate);
                                _db.SaveChanges();
                            }

                            redemptionToUpdate.SetAsProcessed();
                            _db.SaveChanges();
                        }
                    }

                    trasaction.Commit();
                }

                return RedirectToAction(nameof(ProcessRewardList));
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        private bool IsRewardEnough(Redemption redemption)
        {
            foreach (var item in redemption.RedemptionItems)
            {
                //int totalStock = GetAvailableRewards();
                //if (item.Quantity >= totalStock)
                //    return false;
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
