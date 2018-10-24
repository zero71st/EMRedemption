using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Redemption:BaseEntity
    {
        public string DealerName { get; set; }
        public string Email { get; set; }
        public int CouponPrice { get; set; }
        public int Quantity { get; set; }

        public DateTime FetchTime { get; set; }
        public virtual List<Coupon> Coupons { get; set; }
    }
}
