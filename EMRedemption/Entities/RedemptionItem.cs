using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class RedemptionItem:BaseEntity
    {
        public string RewardCode { get; set; }
        public string RewardName { get; set; }
        public int Quantity { get; set; }
        public int Points { get; set; }

        public int RedemptionId { get; set; }
        public virtual Redemption Redemption { get; set; }

        public virtual List<Reward> Rewards { get; set; }

        public RedemptionItem()
        {
            Rewards = new List<Reward>();
        }
    }
}
