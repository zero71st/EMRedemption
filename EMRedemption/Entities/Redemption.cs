using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Redemption:BaseEntity
    {
        [MaxLength(10)]
        public string TransactionID { get; set; }
        [MaxLength(100)]
        public string RetailerName { get; set; }
        [MaxLength(100)]
        public string RetailerStoreName { get; set; }
        [MaxLength(100)]
        public string RetailerEmailAddress { get; set; }
        [MaxLength(20)]
        public string RetailerPhoneNumber { get; set; }
        public RedemptionStatus Status { get; set; }
        public DateTime RedeemDateTime { get; set; }
        [MaxLength(30)]
        public string FetchBy { get; set; }
        public DateTime FetchDateTime { get; set; }

        public virtual List<RedemptionItem> RedemptionItems { get; set; }

        public Redemption()
        {
            RedemptionItems = new List<RedemptionItem>();
            Status = RedemptionStatus.Unprocess;
        }
  
        public void SetAsProcessed()
        {
            Status = RedemptionStatus.Processed;
        }

        public void SetAsDeliveredSuccessful()
        {
            Status = RedemptionStatus.DeliveredSuccessful;
        }

        public void SetAsUndeliverSuccessful()
        {
            Status = RedemptionStatus.UndeliverSuccessful;
        }
    }
}
