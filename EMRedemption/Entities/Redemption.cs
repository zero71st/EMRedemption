using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Redemption:BaseEntity
    {
        public string TransactionID { get; set; }
        public string RetailerName { get; set; }
        public string RetailerStoreName { get; set; }
        public string RetailerEmailAddress { get; set; }
        public string RetailerPhoneNumber { get; set; }
        public RedemptionStatus Status { get; set; }
        public DateTime RedeemDateTime { get; set; }
        public string FetchBy { get; set; }
        public DateTime FetchDateTime { get; set; }

        public virtual List<RedemptionItem> RedemptionItems { get; set; }

        public Redemption()
        {
            RedemptionItems = new List<RedemptionItem>();
            Status = RedemptionStatus.New;
        }
  
        public void SetAsProcessStock()
        {
            Status = RedemptionStatus.ProcessStock;
        }

        public void SetAsEmailSended()
        {
            Status = RedemptionStatus.EmailSended;
        }
    }
}
