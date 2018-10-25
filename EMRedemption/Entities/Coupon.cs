using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Coupon:BaseEntity
    {
        public string Code { get; set; }
        public int Price { get; set; }
        public DateTime ExpireDate { get; set; }
        public string AddBy { get; set; }
        public DateTime AddDate { get; set; }

        public int? RedemptionId { get; set; }
        public virtual Redemption Redemption { get; set; }

        public Coupon()
        {
            AddDate = DateTime.Now;
        }

        public Coupon(string code,int price,DateTime expireDate,string addBy):this()
        {
            Code = code;
            Price = price;
            ExpireDate = expireDate;
            AddBy = addBy;
        }
    }
}
