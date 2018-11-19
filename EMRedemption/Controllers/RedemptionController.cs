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
using Microsoft.Extensions.Logging;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMRedemption.Controllers
{
    public class RedemptionController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _mailSender;
        private readonly ITDesCryptoService _cryptoService;
        private readonly ILogger<RedemptionController> _logger;

        public RedemptionController(ApplicationDbContext db,
                                    IConfiguration configuraton,
                                    IEmailSender sender,
                                    ILogger<RedemptionController> logger,
                                    ITDesCryptoService cryptoService)
        {
            _db = db;
            Configuration = configuraton;
            _mailSender = sender;
            _cryptoService = cryptoService;
            _logger = logger;
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
        [ValidateAntiForgeryToken]
        public IActionResult SendEmail(int id)
        {
            var redemptions = _db.Redemptions
                                 .Include(r => r.RedemptionItems)
                                    .ThenInclude(ri=> ri.Rewards)
                                 .Where(r => r.Status == RedemptionStatus.Processed)
                                 .ToList();

            if (redemptions.Count > 0)
            {
                foreach (var redemption in redemptions)
                {
                    try
                    {
                        var body = CreateMailBody(redemption);

                        _mailSender.SendEmailAsync(redemption.RetailerEmailAddress, "Deliver Voucher Code", body.ToString());

                        redemption.SetAsDeliveredSuccessful();
                        _db.Update(redemption);
                        _db.SaveChanges();

                        _logger.LogInformation("Transaction ID:{0} was sent E-mail by {1} successful", redemption.TransactionID, User.Identity.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Can not send E-mail to Transaction ID:{0} by {1} with problem {2}", redemption.TransactionID, User.Identity.Name, ex.Message);
                    }
                }

                return RedirectToAction(nameof(SendEmailList), "Redemption", new { @filterName = RedemptionProcess.Processed });
            }

            return View();
        }

        private StringBuilder CreateMailBody(Redemption redemption)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<body style='font-family:Cordia New;font-size:14pt'><p>เรียน ลูกค้า/ ผู้ใช้บริการแอพพลิเคชั่น  Mobil Loyalty club</p>");
            sb.AppendLine("<p></p><p></p>");
            sb.AppendLine("<p>ขอบคุณที่ใช้บริการสั่งผลิตภัณฑ์โมบิลผ่านแอพพลิเคชั่น Mobil Loyalty กรุณาอ้างอิงรหัสเงินสดลาซาด้าได้จากที่นี่</p>");

            foreach (var item in redemption.RedemptionItems)
            {
                foreach (var reward in item.Rewards)
                {
                    sb.AppendLine($"<p style='color:blue;'>{_cryptoService.Decrypt(reward.SerialNo).Trim()}</p>");
                }
            }

            sb.AppendLine("<p>อย่าลืม ยิ่งขาย/ซื้อโมบิลมาก ยิ่งได้มาก</p>");
            sb.AppendLine("<p></p><p></p><p></p>");
            sb.AppendLine("<p>ข้อสงสัยเกี่ยวกับการใช้รหัสเงินสดลาซาด้า หรือบริการอื่นๆของลาซาด้า ติดต่อ 02 018 0000 ทุกวันจันทร์- ศุกร์ เวลา 8.00-22.00 น. และวันเสาร์ อาทิตย์ เวลา 9.00-18.00 น.</p>");
            sb.AppendLine("<p></p><p></p>");
            sb.AppendLine("<u><b>ข้อกำหนดเงื่อนไขของรหัสเงินสดจากลาซาด้า</b></u>");
            sb.AppendLine("<ol>");
            sb.AppendLine("<li>ขอสงวนสิทธิ์การใช้รหัสเงินสดได้เพียง 1 ครั้ง ต่อ 1 ท่าน ตลอดรายการส่งเสริมขาย ส่วนต่างของมูลค่าไม่สามารถ แลกเปลี่ยน/ โอน หรือทอนเป็นเงินสดได้</li>");
            sb.AppendLine("<li>รหัสเงินสดนี้ไม่สามารถโอน/เปลี่ยน/แลก/ทอนเป็นเงินสดได้</li>");
            sb.AppendLine("<li>ระยะเวลาการใช้รหัสเงินสดคือ 6 เดือน หลังจากวันที่ได้รับรหัสเงินสดนี้</li>");
            sb.AppendLine("<li>ลูกค้าต้องสมัครสมาชิกและทำการล็อคอินก่อนการใช้รหัสเงินสด</li>");
            sb.AppendLine("<li>รหัสเงินสดนี้ไม่สามารถใช้ร่วมกับรหัสโปรโมชั่นรายการขายอื่นๆของลาซาด้า และไม่สามารถใช้ร่วมกับสินค้าที่ถูกควบคุมราคาด้วยกฎหมาย หรือควบคุมการจัดรายการส่งเสริมการขายจากทางเจ้าของร้านค้าได้</li>");
            sb.AppendLine("<li>รหัสเงินสดนี้ใช้กับการชำระเงินเต็มจำนวนเท่านั้น</li>");
            sb.AppendLine("<li>รหัสเงินสดนี้ไม่สามารถใช้ร่วมกับการสั่งซื้อแบบคอร์ปอเรท และไม่สามารถใช้ได้กับตัวแทนจำหน่าย</li>");
            sb.AppendLine("<li>ลาซาด้าขอสงวนสิทธิ์ในการยกเลิกการสั่งซื้อที่ไม่ตรงตามเงื่อนไขตามที่บริษัทกำหนด อีกทั้งสงวนสิทธิ์ในการเปลี่ยนแปลงเงื่อนไขรายการโดยไม่ต้องแจ้งให้ทราบล่วงหน้า และในกรณีพิพาท คำตัดสินของบริษัทฯ ถือเป็นที่สุด</li>");
            sb.AppendLine("<li>เงื่อนไขการซื้อสินค้า การคืนสินค้า เป็นไปตามที่ www.lazada.co.th กำหนด</li>");
            sb.AppendLine("<li>ข้อมูลเพิ่มเติมเกี่ยวกับการใช้รหัสเงินสด การสั่งซื้อ การคืนสินค้า การเช็ครหัสการส่งสินค้า หรือบริการอื่นๆของลาซาด้า ติดต่อได้ที่ 02-018 0000 ทุกวันจันทร์- ศุกร์ เวลา 8.00-22.00 น. และวันเสาร์ อาทิตย์ เวลา 9.00-18.00 น.</li>");
            sb.AppendLine("</ol>");
            sb.AppendLine("<p></p><p></p><p></p>");
            sb.AppendLine("<p>ขอขอบคุณ</p>");
            sb.AppendLine("<p></p><p></p><p></p>");
            sb.AppendLine("<p><h3>Mobil Loyalty Club</h3></p></body>");

            return sb;
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

                _logger.LogInformation("Transaction ID:{0} was changed status to delivered by {1} successful", redemption.TransactionID, User.Identity.Name);
                return RedirectToAction(nameof(SendEmailList), new { @filterName = RedemptionProcess.DeliveredSuccessful });
            }
            catch (Exception ex)
            {
                _logger.LogError("Can not change status to delivered successful by {0} with problem {1}",User.Identity.Name,ex.Message);
                ModelState.AddModelError("","Can not change status "+ex.Message);
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
        [ValidateAntiForgeryToken]
        public IActionResult ResendEmail(int id)
        {
            var resendMails = _db.Redemptions.Where(c => c.Status == RedemptionStatus.UndeliverSuccessful).ToList();

            foreach (var resend in resendMails)
            {
                try
                {
                    var body = CreateMailBody(resend);

                    _mailSender.SendEmailAsync(resend.RetailerEmailAddress, "Re-deliver Voucher Code",body.ToString());
                    resend.SetAsDeliveredSuccessful();
                    _db.SaveChanges();

                    _logger.LogInformation("Transaction ID:{0} was resend E-mail by {1} successful",resend.TransactionID,User.Identity.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Can not resend Transaction ID:{0} by {1} with problem", resend.TransactionID, User.Identity.Name,ex.Message);
                }
            }

            return RedirectToAction(nameof(SendEmailList), "Redemption", new { @filterName = RedemptionProcess.UndeliverSuccessful });
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

                _logger.LogInformation("Redemptions on:{0} quantity:{1} transactions were loaded to database by {2} successful", model.RedeemDate,redemptions.Count,User.Identity.Name);
                return RedirectToAction(nameof(Retrieve));
            }
            catch (Exception ex)
            {
                _logger.LogError("Can not save redemption on {0} to database by {1} with problem {2}",model.RedeemDate,User.Identity.Name,ex.Message);
                ModelState.AddModelError("", "Can not save redemptions!");
            }

            return View(model);
        }

        private IEnumerable<Reward> GetAvailableRewards()
        {
            return _db.Rewards
                      .Where(rw=> rw.Quantity > 0)
                      .OrderBy(rw => rw.RewardCode)
                      .ThenBy(rw => rw.LotNo)
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
                                   .Include(r=> r.RedemptionItems)
                                        .ThenInclude(ri=> ri.Rewards)
                                   .Where(r => r.Status == RedemptionStatus.Unprocess)
                                   .ToList();

            foreach (var redemption in redemptions)
            {
                bool skip = false;
                foreach (var item in redemption.RedemptionItems)
                {
                    int stock = stockRewards.Where(rw => rw.RewardCode.Equals(item.RewardCode))
                                            .Where(rw=> rw.Quantity > 0)
                                            .Sum(rw => rw.Quantity);

                    if (item.Quantity > stock)
                        skip = true;
                }

                if (skip) continue;

                foreach (var redemptionItem in redemption.RedemptionItems)
                {
                    var rewards = stockRewards.Where(rw => rw.RewardCode.Equals(redemptionItem.RewardCode))
                                              .Where(rw => rw.Quantity > 0)
                                              .OrderBy(rw => rw.LotNo)
                                              .ToList();

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
        [ValidateAntiForgeryToken]
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

                            _logger.LogInformation("Transaction ID:{0} was processed reward by {1} successful ", redemptionToUpdate.TransactionID,User.Identity.Name);
                        }
                    }

                    trasaction.Commit();
                }

                return RedirectToAction(nameof(ProcessRewardList));
            }
            catch (Exception ex)
            {
                _logger.LogError("Can not process reward by {0} with problem {1}", User.Identity.Name,ex.Message);
                ModelState.AddModelError("", "Can not process reward found problem " + ex.Message);
            }

            return View(models);
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
