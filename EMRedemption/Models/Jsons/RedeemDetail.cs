using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.Jsons
{
    public class RedeemDetail
    {
        public int TransactionID { get; set; }
        public string retailerName { get; set; }
        public string retailerStoreName { get; set; }
        public string retailerEmailAddress { get; set; }
        public string retailerAddress { get; set; }
        public string retailerPhoneNumber { get; set; }

        public List<productDetail> productDetails { get; set; }
    }
}
