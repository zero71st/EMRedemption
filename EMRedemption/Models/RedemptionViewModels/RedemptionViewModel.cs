using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using EMRedemption.Entities;

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

        [DisplayName("Redeem Date Time")]
        public DateTime RedeemDateTime { get; set; }

        [DisplayName("Fetch Date Time")]
        public DateTime FetchDateTime { get; set; }

        public List<RedemptionItemViewModel> RedemptionItems { get; set; }

        public RedemptionViewModel()
        {
            RedemptionItems = new List<RedemptionItemViewModel>();
        }
        public RedemptionViewModel(Redemption redemption):this()
        {
            TransactionID = redemption.TransactionID;
            RetailerName = redemption.RetailerName;
            RetailerStoreName = redemption.RetailerStoreName;
            RetailerPhoneNumber = redemption.RetailerPhoneNumber;
            RetailerEmailAddress = redemption.RetailerEmailAddress;
            RedeemDateTime = redemption.RedeemDateTime;
            FetchDateTime = redemption.FetchDateTime;
            int i = 0;
            RedemptionItems.AddRange(redemption.RedemptionItems.Select(ri =>
            {
                i++;
                return new RedemptionItemViewModel()
                {
                    LineNo = i,
                    Id = ri.Id,
                    RewardCode = ri.RewardCode,
                    RewardName = ri.RewardName,
                    Points = ri.Points,
                    Quantity = ri.Quantity,
                };
            }));
        }

        public RedemptionViewModel(int lineNo,Redemption redemption):this(redemption)
        {
            LineNo = lineNo;
        }
    }
}
