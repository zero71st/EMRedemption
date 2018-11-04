using EMRedemption.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RewardViewModels
{
    public class RewardListViewModel
    {
        public List<RewardViewModel> Rewards { get; set; }
        public SelectList Filters { get; set; }
        public string FilterName { get; set; }
        public string Keyword { get; set; }
    }
}
