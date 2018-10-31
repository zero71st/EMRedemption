using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class RedemptionViewModel
    {
        public int Id { get; set; }
        public bool Selected { get; set; }

        [DisplayName("#")]
        public int LineNo { get; set; }

        [DisplayName("Transaction ID")]
        public string TransactionID { get; set; }

        [DisplayName("Retailer Name")]
        public string RetailerName { get; set; }

        [DisplayName("Store Name")]
        public string RetailerStoreName { get; set; }

        [DisplayName("Email Address")]
        public string RetailerEmailAddress { get; set; }

        [DisplayName("Phone Number")]
        public string RetailerPhoneNumber { get; set; }

        [DisplayName("Redeem DateTime")]
        public string RedeemDateTime { get; set; }

        public List<RedemptionItemViewModel> RedemptionItems { get; set; }
    }
}
