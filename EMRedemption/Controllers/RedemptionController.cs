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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMRedemption.Controllers
{
    public class RedemptionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RedemptionController(ApplicationDbContext db,
                                    IConfiguration configuraton)
        {
            _db = db;

            Configuration = configuraton;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var models = _db.Redemptions.ToList().Select(r => new RedemptionViewModel(r));
            return View(models);
        }

        public IConfiguration Configuration { get; }

        [HttpGet]
        public async Task<IActionResult> Retrieve(string retriveDate)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://test.thmobilloyaltyclub.com/api/redeem_voucher_list");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("accessKey", "thai$2R@88"),
                new KeyValuePair<string, string>("startDate", "2018-10-30 09:05 PM"),
                new KeyValuePair<string,string>("endDate","2018-10-30 10:40 PM")
            });

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

            return View(models);
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
