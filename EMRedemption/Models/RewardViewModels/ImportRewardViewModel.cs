using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RewardViewModels
{
    public class ImportRewardViewModel
    {
        public int RewardTypeId { get; set; }
        public SelectList RewardTypes { get; set; }
        public DateTime LotDate { get; set; }
        public string Description { get; set; }
    }
}
