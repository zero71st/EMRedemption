using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class RedemptionItem:BaseEntity
    {
        public string RewardCode { get; set; }
        public int Quantity { get; set; }
        public int Points { get; set; }

        public virtual List<Reward> Rewards { get; set; }
    }
}
