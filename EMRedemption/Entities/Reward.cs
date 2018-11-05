using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Entities
{
    public class Reward:BaseEntity
    {
        [MaxLength(5)]
        public string Code { get; set; }
        public string SerialNo { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpireDate { get; set; }
        [MaxLength(10)]
        public string LotNo { get; set; }
        public string RewardType { get; set; }
        public string AddBy { get; set; }
        public DateTime AddDate { get; set; }

        public int? RedemptionItemId { get; set; }
        public virtual RedemptionItem RedemptionItem { get; set; }

        public Reward()
        {
            AddDate = DateTime.Now;
        }

        public Reward(string code,string serialNo,string description,string rewardType,DateTime expireDate,int quantity,string addBy):this()
        {
            Code = code;
            SerialNo = serialNo;
            Description = description;
            RewardType = rewardType;
            ExpireDate = expireDate;
            Quantity = quantity;
            AddBy = addBy;
        }
    }
}