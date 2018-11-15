using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class RewardType:BaseEntity
    {
        [MaxLength(5)]
        public string Code { get; set; }
        [MaxLength(100)]
        public string RewardName { get; set; }
        [MaxLength(100)]
        public string RewardTypeName { get; set; }
    }
}
