using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using System.Net.Http;
using System.Transactions;


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
        public async Task<object> FunctionHandler(object input, ILambdaContext context)
        {
            DateTime now = new DateTime();
            string startDate = new DateTime(now.Year, now.Month, now.Day).AddDays(20).ToString("yyyy-MM-dd");
            string endDate = new DateTime(now.Year, now.Month, now.Day).ToString("yyyy-MM-dd");

            var client = new HttpClient();

            client.BaseAddress = new Uri("http://test.thmobilloyaltyclub.com/api/redeem_voucher_list");
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("accessKey", "thai$2R@88"),
                new KeyValuePair<string, string>("startDate", "2018-11-01 10:00 AM"),
                new KeyValuePair<string,string>("endDate","2018-11-29 10:00 AM")
            });

            ////client.BaseAddress = new Uri("https://www.thmobilloyaltyclub.com/api/redeem_voucher_list");
            ////var content = new FormUrlEncodedContent(new[]
            ////{
            ////    new KeyValuePair<string, string>("accessKey", "ACT^H9&5#"),
            ////    new KeyValuePair<string, string>("startDate", startDate+" 10:00 AM"),
            ////    new KeyValuePair<string,string>("endDate",endDate+" 10:00 AM")
            ////});
            
            try
            {
                var resp = await client.PostAsync("", content);

                var jsonString = await resp.Content.ReadAsStringAsync();

                var jsons = JsonConvert.DeserializeObject<JsonResponse>(jsonString);

                if (jsons.redeemDetails == null)
                    return null;

                var connStr = "server=pongsatornoffice.cqttbtdz5ct1.ap-southeast-1.rds.amazonaws.com; Port=3306; Database=EMRedemptionDB; Uid=thel3oat0142; Pwd=thel3oat;convert zero datetime=True;SslMode=none;";

                using (TransactionScope scope = new TransactionScope())
                {
                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    {
                        foreach (var model in jsons.redeemDetails)
                        {
                            conn.Open();

                            string sql = "INSERT INTO Redemptions(TransactionID,RetailerName,RetailerStoreName,RetailerEmailAddress,RetailerPhoneNumber,RedeemDateTime,FetchBy,FetchDateTime)" +
                                                         "VALUES (@transactionID,@retialerName,@retialerStoreName,@retailerEmailAddress,@retailerPhoneNumber,@redeemDateTime,@fetchBy,@fetchDateTime)";

                            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@transactionID", model.TransactionID);
                                cmd.Parameters.AddWithValue("@retialerName", model.retailerName);
                                cmd.Parameters.AddWithValue("@retialerStoreName", model.retailerStoreName);
                                cmd.Parameters.AddWithValue("@retailerEmailAddress", model.retailerEmailAddress);
                                cmd.Parameters.AddWithValue("@retailerPhoneNumber", model.retailerPhoneNumber);
                                cmd.Parameters.AddWithValue("@redeemDateTime", model.RedeemDateTime.ToString("yyyy-MM-dd H:mm:ss"));
                                cmd.Parameters.AddWithValue("@fetchBy","AWS Lamda");
                                cmd.Parameters.AddWithValue("@fetchDateTime", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));

                                cmd.ExecuteNonQuery();

                                foreach (var item in model.productDetails)
                                {
                                    sql = "INSERT INTO RedemptionItems(RedemptionId,RewardCode,RewardName,Points,Quantity) VALUES (@redemptionID,@rewardCode,@rewardName,@points,@quantity)";
                                    using (MySqlCommand cmd2 = new MySqlCommand(sql, conn))
                                    {
                                        cmd2.Parameters.AddWithValue("@redemptionID", cmd.LastInsertedId);
                                        cmd2.Parameters.AddWithValue("@rewardCode", item.productCode);
                                        cmd2.Parameters.AddWithValue("@rewardName", item.productName);
                                        cmd2.Parameters.AddWithValue("@points", item.points);
                                        cmd2.Parameters.AddWithValue("@quantity", item.quantity);

                                        cmd2.ExecuteNonQuery();
                                    }
                                }
                            }
                            conn.Close();
                        }
                    }

                    scope.Complete();

                }

                context.Logger.LogLine("Insert completed!");
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("Insert data failed! "+ex);
            }

            return input;
        }
    }

    public class JsonResponse
    {
        public int responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<RedeemDetail> redeemDetails { get; set; }
    }

    public class RedeemDetail
    {
        public string TransactionID { get; set; }
        public string retailerName { get; set; }
        public string retailerStoreName { get; set; }
        public string retailerEmailAddress { get; set; }
        public string retailerAddress { get; set; }
        public string retailerPhoneNumber { get; set; }
        public DateTime RedeemDateTime { get; set; }

        public List<productDetail> productDetails { get; set; }
    }

    public class productDetail
    {
        public string productCode { get; set; }
        public string productName { get; set; }
        public int points { get; set; }
        public int quantity { get; set; }
    }
}
