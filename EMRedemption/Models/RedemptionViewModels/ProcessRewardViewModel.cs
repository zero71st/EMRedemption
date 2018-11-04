using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class ProcessRewardViewModel
    {
        public string RewardCode { get; set; }
        public string RewardName { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }
        public int Balance { get; set; }
    }
}
