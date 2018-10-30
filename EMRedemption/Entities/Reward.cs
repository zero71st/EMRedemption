using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Reward:BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime ExpireDate { get; set; }
        public string AddBy { get; set; }
        public DateTime AddDate { get; set; }

        public int? RedemptionId { get; set; }
        public virtual Redemption Redemption { get; set; }

        public Reward()
        {
            AddDate = DateTime.Now;
        }

        public Reward(string code,string description,DateTime expireDate,string addBy):this()
        {
            Code = code;
            Description = description;
            ExpireDate = expireDate;
            AddBy = addBy;
        }
    }
}