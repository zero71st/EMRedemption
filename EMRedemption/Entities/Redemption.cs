using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Redemption:BaseEntity
    {
        public DateTime RedeemDate { get; set; }
        public string DealerName { get; set; }
        public string Email { get; set; }
        public int CouponPrice { get; set; }
        public int Quantity { get; set; }
        public RedemptionStatus Status { get; set; }

        public DateTime FetchTime { get; set; }
        public virtual List<Coupon> Coupons { get; set; }

        public Redemption()
        {
            Status = RedemptionStatus.New;
        }

        public Redemption(DateTime redeemDate,string dealerName,string email,int couponPrice,int quantity,DateTime fetchTime):this()
        {
            FetchTime = fetchTime;
            RedeemDate = redeemDate;
            DealerName = dealerName;
            Email = email;
            CouponPrice = couponPrice;
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
