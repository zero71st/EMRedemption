using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class RedemptionListViewModel
    {
        public List<RedemptionViewModel> Redemptions { get; set; }
        public SelectList Filter { get; set; }
        public string FilterName { get; set; }
        public String Keyword {get;set;}
    }
}
