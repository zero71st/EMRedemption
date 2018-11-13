using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class ProcessRewardViewModel
    {
        public int RewardId { get; set; }
        public int RedemptionId { get; set; }
        public int RedemptionItemId { get; set; }
        public int Quantity { get; set; }
    }
}
