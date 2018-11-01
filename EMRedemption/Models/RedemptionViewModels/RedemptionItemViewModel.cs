using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class RedemptionItemViewModel
    {
        public int Id { get; set; }
        
        [DisplayName("#")]
        public int LineNo { get; set; }

        [DisplayName("Code")]
        public string RewardCode {get;set;}

        [DisplayName("Points")]
        public int Points { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }
    }
}
