using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMRedemption.Models.RedemptionViewModels;
using MySql.Data;
using System.Net.Http;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using EMRedemption.Models.Jsons;
using EMRedemption.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMRedemption.Controllers
{
    public class RedemptionController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RedemptionController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var models = _db.Redemptions.ToList().Select(r => new RedemptionViewModel(r));
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

            var resp = await client.PostAsync("",content);

            var jsonString = await resp.Content.ReadAsStringAsync();

            var objects = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

            int i = 0;
            //var models = objects.redeemDetails.Select(r => {
            //    i++;
            //    return new RedemptionViewModel
            //    {
            //        LineNo = i,
            //        TransactionID = r.TransactionID,
            //        RetailerName = r.retailerName,
            //        RetailerStoreName = r.retailerStoreName,
            //        RetailerEmailAddress = r.retailerEmailAddress,
            //        RetailerPhoneNumber = r.retailerPhoneNumber,
            //    };
            //});
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
                    item.Points = detail.points;
                    item.Quantity = detail.quantity;
                    redemptionItems.Add(item);
                }

                redemption.RedemptionItems.AddRange(redemptionItems);

                models.Add(redemption);
            }

            //var connStr = ConfigurationManager.AppSettings["maria_connection"];
            var connStr = "server=pongsatornoffice.cqttbtdz5ct1.ap-southeast-1.rds.amazonaws.com; Port=3306; Database=l3oatoffice; Uid=thel3oat0142; Pwd=thel3oat;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                foreach (var model in models)
                {
                    conn.Open();

                    string sql = String.Format("INSERT INTO Redemptions(TrasactionID,RetailerName,RetailerStoreName,RetailerEmailAddress,RetailerPhoneNumber,RedeemDateTime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')", model.TransactionID,model.RetailerName,model.RetailerStoreName,model.RetailerEmailAddress,model.RetailerPhoneNumber,model.RedeemDateTime);
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        var rows = cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }


            return View(models.OrderBy(m=> m.RedeemDateTime).ThenBy(m=> m.TransactionID));
        }
    }
}
