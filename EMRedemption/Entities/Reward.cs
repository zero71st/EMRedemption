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
        public string RewardCode { get; set; }
        [MaxLength(100)]
        public string RewardName { get; set; }
        [MaxLength(100)]
        public string RewardTypeName { get; set; }
        [MaxLength(50)]
        public string SerialNo { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpireDate { get; set; }
        [MaxLength(10)]
        public string LotNo { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        [MaxLength(30)]
        public string AddBy { get; set; }
        public DateTime AddDate { get; set; }

        public int? RedemptionItemId { get; set; }
        public virtual RedemptionItem RedemptionItem { get; set; }

        public int RewardTypeId { get; set; }
        public virtual RewardType RewardType { get; set; }

        public Reward()
        {
            AddDate = DateTime.Now;
        }

        public Reward(string rewardCode,string rewardName,string serialNo,string description,string rewardTypeName,DateTime expireDate,int quantity,DateTime lotNo,string addBy):this()
        {
            RewardCode = rewardCode;
            RewardName = rewardName;
            RewardTypeName = rewardTypeName;
            SerialNo = serialNo;
            Description = description;
            ExpireDate = expireDate;
            Quantity = quantity;
            LotNo = lotNo.ToString("yyyy-MM-dd");
            AddBy = addBy;
        }
    }
}