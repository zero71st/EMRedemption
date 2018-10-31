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

        public async Task<IActionResult> CallApi()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://test.thmobilloyaltyclub.com/api/redeem_voucher_list");

            var content = new FormUrlEncodedContent(new[]
             {
                new KeyValuePair<string, string>("accessKey", "thai$2R@88"),
                new KeyValuePair<string, string>("startDate", "2018-10-30"),
                new KeyValuePair<string,string>("endDate","2018-10-30")
             });

            var resp = await client.PostAsync("",content);

            var jsonString = await resp.Content.ReadAsStringAsync();

            var jsonObjects = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

            var models = jsonObjects.redeemDetails.Select(r => new RedemptionViewModel
            {
                RetailerName = r.retailerName,
                RetailerStoreName = r.retailerStoreName,
                RetailerEmailAddress = r.retailerEmailAddress,
                RetailerPhoneNumber = r.retailerPhoneNumber
            });

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

            return View(models);
        }
    }
}
