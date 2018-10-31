using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.Jsons
{
    public class JsonResponse
    {
        public int responseCode { get; set; }
        public string responseMessage { get; set; }
        public List<RedeemDetail> redeemDetails { get; set; }
    }
}
