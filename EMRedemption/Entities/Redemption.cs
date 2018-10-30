using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Redemption:BaseEntity
    {
        public string RetailerName { get; set; }
        public string RetailerStoreName { get; set; }
        public string RetailerEmailAddress { get; set; }
        public string RetailerPhoneNumber { get; set; }
        public int RewardCode { get; set; }
        public int Quantity { get; set; }
        public RedemptionStatus Status { get; set; }
        public DateTime RedeemDateTime { get; set; }
        public DateTime FatchDateTime { get; set; }

        public virtual List<Reward> Rewards { get; set; }

        public Redemption()
        {
            Status = RedemptionStatus.New;
        }

        public Redemption(DateTime redeemDate,string dealerName,string email,int couponPrice,int quantity,DateTime fetchTime):this()
        {
            FatchDateTime = fetchTime;
            RedeemDateTime = redeemDate;
            RetailerName = dealerName;
            RetailerEmailAddress = email;
            RewardCode = couponPrice;
            Quantity = quantity;
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
