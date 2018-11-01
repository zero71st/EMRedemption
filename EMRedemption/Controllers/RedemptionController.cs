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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMRedemption.Controllers
{
    public class RedemptionController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {

            return View();
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
            //var connStr = "server=exxon-db-server.mariadb.database.azure.com; Port=3306; Database=EMRedemptionDb; Uid=kasem@exxon-db-server; Pwd=abc123!@#;";
            //using (MySqlConnection conn = new MySqlConnection(connStr))
            //{
            //    ////string sql = "INSERT INTO todo(id,userid,title,completed) VALUES ("+todo.id+","+todo.userid+",'"+todo.title+"',true)";
            //    //string sql = String.Format("INSERT INTO todo(id,userid,title,completed) VALUES ({0},{1},'{2}',{3})", todo.id, todo.userid, todo.title, todo.completed);

            //    //conn.Open();
            //    //using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            //    //{
            //    //    var rows = cmd.ExecuteNonQuery();
            //    //    log.LogInformation($"{rows} efffected");
            //    //}
            //    //conn.Close();
            //}

            return View(models.OrderBy(m=> m.RedeemDateTime).ThenBy(m=> m.TransactionID));
        }
    }
}
