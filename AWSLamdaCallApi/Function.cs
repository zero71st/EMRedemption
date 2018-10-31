using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using System.Net.Http;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLamdaCallApi
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            //var client = new HttpClient();
            //HttpContent content = new HttpContent();

            //var resp = client.PostAsync("http://test.thmobilloyaltyclub.com/api/redeem_voucher_list",content);

            //var jsonString = resp.Content.ReadAsStringAsync();

            //var todo = JsonConvert.DeserializeObject<jsonString>(jsonString);

            //var connStr = ConfigurationManager.AppSettings["maria_connection"];
            var connStr = "Server=mariadb-server.mariadb.database.azure.com; Port=3306; Database=redeemdb; Uid=kasem@mariadb-server; Pwd=abc123!@#;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                ////string sql = "INSERT INTO todo(id,userid,title,completed) VALUES ("+todo.id+","+todo.userid+",'"+todo.title+"',true)";
                //string sql = String.Format("INSERT INTO todo(id,userid,title,completed) VALUES ({0},{1},'{2}',{3})", todo.id, todo.userid, todo.title, todo.completed);

                //conn.Open();
                //using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                //{
                //    var rows = cmd.ExecuteNonQuery();
                //    log.LogInformation($"{rows} efffected");
                //}
                //conn.Close();
            }
            return input?.ToUpper();
        }
    }

    public class RedeemDetials
    {
        public int TransactionID { get; set; }
        public string retailerName { get; set; }
        public string retailerStoreName { get; set; }
        public string retailerEmailAddress { get; set; }
        public string retailerAddress { get; set; }
        public string retailerPhoneNumber { get; set; }

        public List<productDetail> productDetails { get; set; }
    }

    public class productDetail
    {
        public string productName { get; set; }
        public int points { get; set; }
        public int quantity { get; set; }
    }
}
