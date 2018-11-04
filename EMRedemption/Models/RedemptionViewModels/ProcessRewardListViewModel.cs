using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class ProcessRewardListViewModel
    {
        public int RedemptionTotal { get; set; }
        public List<ProcessRewardViewModel> ProcessRewards { get; set; }

        public ProcessRewardListViewModel()
        {
            ProcessRewards = new List<ProcessRewardViewModel>();
        }
    }
}
