using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class ConfirmToStoreViewModel
    {
        [DisplayName("Redeem Date")]
        public string RedeemDate { get; set; }
        [DisplayName("Transaction(s)")]
        public int Quantity { get; set; }

        public ConfirmToStoreViewModel()
        {

        }

        public ConfirmToStoreViewModel(string redeemDate,int quantity)
        {
            RedeemDate = redeemDate;
            Quantity = quantity;
        }
    }
}
