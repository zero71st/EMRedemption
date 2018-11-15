using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class RedemptionItem:BaseEntity
    {
        [MaxLength(5)]
        public string RewardCode { get; set; }
        [MaxLength(50)]
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
