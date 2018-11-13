using EMRedemption.Entities;
using EMRedemption.Models.RewardViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class ProcessRewardListViewModel
    {
        public int RedemptionTotal { get; set; }
        public List<Redemption> Redemptions { get; set; }

        public ProcessRewardListViewModel()
        {
            Redemptions = new List<Redemption>();
        }
    }
}
