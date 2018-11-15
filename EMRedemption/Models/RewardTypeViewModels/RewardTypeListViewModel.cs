using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RewardTypeViewModels
{
    public class RewardTypeListViewModel
    {
        public List<RewardTypeViewModel> RewardTypes { get; set; }
        public string Keyword { get; set; }
    }
}
