using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Models.RedemptionViewModels
{
    public class SendEmailViewModel
    {
        public string SendType { get; set; }
        public DateTime SendDateTime { get; set; }
        public int TotalRedemptions { get; set; }
    }
}
