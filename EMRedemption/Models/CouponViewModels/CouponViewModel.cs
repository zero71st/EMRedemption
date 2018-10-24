using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.CouponViewModels
{
    public class CouponViewModel
    {
        public int Id { get; set; }

        public int LineNo { get; set; }

        [DisplayName("Coupon Code")]
        public string Code { get; set; }

        [DisplayName("Coupon Price")]
        public int Price { get; set; }

        [DisplayName("Expire Date")]
        public DateTime ExpireDate { get; set; }

        public string AddBy { get; set; }
        public DateTime AddDate { get; set; }

        public bool IsUsed { get; set; }
    }
}
