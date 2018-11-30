using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Net.Http;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambda3
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object FunctionHandler(object input, ILambdaContext context)
        {
            DateTime now = new DateTime();
            string startDate = new DateTime(now.Year, now.Month, now.Day).AddDays(20).ToString("yyyy-MM-dd");
            string endDate = new DateTime(now.Year, now.Month, now.Day).ToString("yyyy-MM-dd");

            var client = new HttpClient();

            client.BaseAddress = new Uri("http://test.thmobilloyaltyclub.com/api/redeem_voucher_list");
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("accessKey", "thai$2R@88"),
                new KeyValuePair<string, string>("startDate", startDate+" 10:00 AM"),
                new KeyValuePair<string,string>("endDate",endDate+" 10:00 AM")
            });


            return input;
        }
    }
}
