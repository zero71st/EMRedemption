using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Redemption:BaseEntity
    {
        public int TrasactionID { get; set; }
        public string RetailerName { get; set; }
        public string RetailerStoreName { get; set; }
        public string RetailerEmailAddress { get; set; }
        public string RetailerPhoneNumber { get; set; }
        public RedemptionStatus Status { get; set; }
        public DateTime RedeemDateTime { get; set; }
        public DateTime FatchDateTime { get; set; }

        public Redemption()
        {
            Status = RedemptionStatus.New;
        }

        public Redemption(DateTime redeemDateTime,string retialerName,string email,DateTime fetchTime):this()
        {
            RedeemDateTime = redeemDateTime;
            RetailerEmailAddress = email;
            FatchDateTime = fetchTime;
            RetailerName = retialerName;
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
