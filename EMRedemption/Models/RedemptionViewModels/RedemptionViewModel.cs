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

        public int LineNo { get; set; }

        [DisplayName("Redeem Date")]
        public DateTime RedeemDate { get; set; }

        [DisplayName("Dealer Name")]
        public string DealerName { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DisplayName("Coupon Price")]
        public int CouponPrice { get; set; }

        public int Quantity { get; set; }

    }
}
