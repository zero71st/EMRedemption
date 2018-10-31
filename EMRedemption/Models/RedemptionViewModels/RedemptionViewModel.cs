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

        [DisplayName("#")]
        public int LineNo { get; set; }

        [DisplayName("Redeem DateTime")]
        public DateTime RedeemDateTime { get; set; }

        [DisplayName("Dealer Name")]
        public string DealerName { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        public List<RedemptionItemViewModel> RedemptionItems { get; set; }
    }
}
